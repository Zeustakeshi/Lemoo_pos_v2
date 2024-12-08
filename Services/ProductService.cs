using CloudinaryDotNet;
using Lemoo_pos.Data;
using Lemoo_pos.Areas.Api.Dto;
using Lemoo_pos.Models.Entities;
using Lemoo_pos.Models.ViewModels;
using Lemoo_pos.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

using Lemoo_pos.Helper;
using Nest;
using Lemoo_pos.Common.Enums;
using MassTransit;
using Lemoo_pos.Messages;

namespace Lemoo_pos.Services
{
    public class ProductService : IProductService
    {
        private readonly ICloudinaryService _cloudinaryService;
        private readonly AppDbContext _db;
        private readonly ISessionService _sessionService;
        private readonly IInventoryService _inventoryService;
        private readonly IElasticsearchService _elasticsearchService;
        private readonly IPublishEndpoint _publishEndpoint;

        public ProductService(
            ICloudinaryService cloudinaryService,
            AppDbContext db,
            ISessionService sessionService,
            IInventoryService inventoryService,
            IElasticsearchService elasticsearchService,
             IPublishEndpoint publishEndpoint
        )
        {
            _cloudinaryService = cloudinaryService;
            _db = db;
            _inventoryService = inventoryService;
            _sessionService = sessionService;
            _elasticsearchService = elasticsearchService;
            _publishEndpoint = publishEndpoint;
        }


        public List<ProductResponseViewModel> GetAllProduct()
        {

            long storeId = _sessionService.GetStoreIdSession();

            List<Product> products = [.. _db.Products
                .Where(product => product.Store.Id == storeId && product.IsDeleted == false)
                .Include(p => p.Variants)
                .Include(p => p.Brand)
                .Include(p => p.Category)
             ];

            List<ProductResponseViewModel> productResponses = [];

            foreach (Product product in products)
            {
                productResponses.Add(new()
                {
                    Id = product.Id,
                    Name = product.Name,
                    Category = product.Category,
                    Image = product.Image,
                    VariantCount = product.Variants.Count,
                    BrandName = product.Brand?.Name,
                    CreatedAt = product.CreatedAt,
                    UpdatedAt = product.UpdatedAt
                });
            }

            return productResponses;
        }

        public async Task CreateProduct(CreateProductViewModel productData, IFormFile image)
        {
            // Get the store and staff information from the session
            Store store = _sessionService.GetStoreSession();
            Staff staff = _sessionService.GetStaffSession();

            // Create a new product entity based on the provided data
            Product product = new()
            {
                Name = productData.Name,
                Unit = productData.Unit,
                Description = productData.Description,
                Store = store,
            };

            // Check if there is an image provided for the product
            if (image != null)
            {
                // Upload the image to Cloudinary and get the image URL
                string imageUrl = await _cloudinaryService.UploadImageAsync(image, "/products", _cloudinaryService.GenerateImageId(product.Id.ToString()));
                product.Image = imageUrl;
            }
            else
            {
                // Use a default image if no image is provided
                product.Image = "http://res.cloudinary.com/dymmvrufy/image/upload/v1732958992/lemoo_pos/products/cgvy4owbou09tb6okwvu.svg";
            }

            try
            {
                // Add the product to the database
                _db.Products.Add(product);
            }
            catch (Exception ex)
            {
                // Log any errors during the product addition
                await Console.Out.WriteLineAsync(ex.Message);
                throw;
            }

            // Set the category of the product if provided
            if (productData.CategoryId != null)
            {
                ProductCategory category = _db.ProductCategories.Single(category => category.Id == productData.CategoryId);
                if (category != null)
                {
                    product.Category = category;
                    product.CategoryId = category.Id;
                }
            }



            // Create a list of attribute values to be associated with the product
            var attributeValues = new List<(string Id, ProductAttributeValue attributeValue)>();

            // Process the attributes provided for the product
            foreach (var attribute in productData.Attributes)
            {
                // Create a new product attribute
                var productAttribute = new ProductAttribute()
                {
                    Name = attribute.Name
                };

                // Add the attribute to the database
                _db.ProductAttributes.Add(productAttribute);

                // Process each attribute value
                foreach (var value in attribute.Values)
                {
                    var productAttributeValue = new ProductAttributeValue()
                    {
                        Attribute = productAttribute,
                        Value = value.Name,
                    };

                    // Add the attribute value to the database
                    _db.ProductAttributeValues.Add(productAttributeValue);

                    // Save the attribute value for later association with product variants
                    attributeValues.Add((value.Id, attributeValue: productAttributeValue));
                }
            }

            // Fetch the branches for the product based on the provided branch IDs
            List<Branch> branches = _db.Branches.Where(branch => productData.Branches.Contains(branch.Id)).ToList();

            // Process each product variant
            foreach (var variant in productData.Variants)
            {
                // Create a new product variant entity
                ProductVariant productVariant = new()
                {
                    BarCode = variant.BarCode,
                    SellingPrice = Convert.ToInt32(variant.SellingPrice),
                    CostPrice = Convert.ToInt32(variant.CostPrice),
                    SkuCode = variant.SkuCode,
                    Product = product,
                    ProductId = product.Id,
                    AllowSale = variant.AllowSale,
                    Image = product.Image,
                    AllowNegativeInventory = productData.AllowNegativeInventory
                };

                // Add the product variant to the database and get the added variant
                var newProductVariant = _db.ProductVariants.Add(productVariant).Entity;

                _db.SaveChanges();
                // Process the inventory for each branch in parallel
                // This can be done in parallel to improve performance if there are multiple branches
                foreach (var branch in branches)
                {
                    try
                    {
                        // Create inventory for the product variant in the branch
                        // _inventoryService.CreateInventory(
                        //     newProductVariant,
                        //     branch,
                        //     variant.Quantity,
                        //     variant.Quantity,
                        //     staff: staff
                        // );
                        await _publishEndpoint.Publish(new InitInventoryMessage()
                        {
                            Available = variant.Quantity,
                            Quantity = variant.Quantity,
                            BranchId = branch.Id,
                            ProductVariantId = newProductVariant.Id,
                            StaffId = staff.Id
                        });
                    }
                    catch (Exception ex)
                    {
                        // Log any errors during inventory creation
                        await Console.Out.WriteLineAsync(ex.Message);
                        throw new Exception("Lưu kho thất bại");
                    }
                }

                // Process the variant's attribute values and associate them with the variant
                List<string> variantNames = [];

                foreach (var attributeId in variant.AttributeValues)
                {
                    // Find the product attribute value associated with the variant's attribute
                    ProductAttributeValue value = attributeValues.FirstOrDefault(a => a.Id.Equals(attributeId)).attributeValue;
                    if (value == null) continue;

                    // Add the attribute value to the product variant
                    _db.ProductVariantAttributes.Add(new()
                    {
                        Variant = productVariant,
                        AttributeValue = value
                    });

                    // Add the attribute value to the list of variant names
                    variantNames.Add(value.Value);
                }

                // Save the product variant information to Elasticsearch
                await _publishEndpoint.Publish(new SaveSearchProductMessage()
                {
                    Id = product.Id,
                    Branches = productData.Branches,
                    Name = product.Name,
                    Price = variant.SellingPrice,
                    Quantity = variant.Quantity,
                    SkuCode = variant.SkuCode,
                    StoreId = store.Id,
                    VariantName = String.Join("-", variantNames),
                    Image = product.Image,
                    VariantId = newProductVariant.Id,
                    AllowNegativeInventory = productData.AllowNegativeInventory,
                    Keyword = LanguageHelper.RemoveVietnameseTones(product.Name + " " + String.Join("-", variantNames)),
                });
            }

            // Commit the changes to the database
            _db.SaveChanges();


            // Set the brand of the product if provided
            if (productData.BrandId != null)
            {
                await _publishEndpoint.Publish(new UpdateProductBrandMessage()
                {
                    ProductId = product.Id,
                    BrandId = productData.BrandId ?? throw new Exception("Invalid product brand")
                });
            }



        }


        public List<ProductVariantResponseViewModel> GetAllVariants(long productId)
        {
            List<ProductVariant> variants = [.. _db.ProductVariants
                .Include(v => v.AttributeValues)
                .ThenInclude(av => av.AttributeValue)
                .Include(v => v.Product)
                .Where(variant => variant.ProductId == productId)
            ];

            List<ProductVariantResponseViewModel> variantResponses = [];


            foreach (var variant in variants)
            {
                variantResponses.Add(ConvertProductVariantToProductVariantResponse(variant));
            }

            return variantResponses;
        }

        public ProductVariantDetailResponseViewModel GetProductVariantByIdAndProductID(long variantId, long productId)
        {
            long storeId = _sessionService.GetStoreIdSession();

            var variant = _db.ProductVariants
                .Include(v => v.AttributeValues)
                .ThenInclude(av => av.AttributeValue)
                .Include(v => v.Product)
                .Include(v => v.Inventories)
                .ThenInclude(inventory => inventory.Branch)
                .Where(v => v.Id == variantId && v.ProductId == productId && v.Product.Store.Id == storeId).Single();

            List<Branch> branches = [.. _db.Branches.Where(branch => branch.StoreId == storeId)];

            List<InventoryInfoViewModel> inventories = [];

            foreach (var inventory in variant.Inventories)
            {
                inventories.Add(new()
                {
                    Id = inventory.Id,
                    BranchName = inventory.Branch.Name,
                    Available = inventory.Available,
                    Quantity = inventory.Quantity
                }
                );
            }

            ProductVariantDetailResponseViewModel response = new()
            {
                Id = variant.Id,
                Product = variant.Product,
                BarCode = variant.BarCode,
                CostPrice = variant.CostPrice,
                SellingPrice = variant.SellingPrice,
                CreatedAt = variant.CreatedAt,
                UpdatedAt = variant.UpdatedAt,
                SkuCode = variant.SkuCode,
                Name = String.Join("-", variant.AttributeValues.Select(a => a.AttributeValue.Value)),
                Image = variant.Image,
                Variants = GetAllVariants(productId).Where(v => v.Id != variantId).ToList(),
                Inventories = inventories
            };

            return response;
        }

        public void DeleteProduct(long productId)
        {
            Product product = _db.Products.Single(p => p.Id == productId);
            if (product == null)
            {
                throw new Exception("Không tìm thấy sản phẩm này");
            }


            product.IsDeleted = true;

            _elasticsearchService.DeleteDocumentById("products", productId.ToString());

            _db.Products.Update(product);

            _db.SaveChanges();
        }

        public async Task UpdateProductVariant(long productId, long variantId, UpdateProductVariantViewModel model, IFormFile image)
        {
            long storeId = _sessionService.GetStoreIdSession();

            var variant = _db.ProductVariants
                .Include(v => v.Product)
                .Where(v => v.Id == variantId && v.ProductId == productId && v.Product.Store.Id == storeId)
                .Single() ?? throw new Exception("Không tìm thấy biến thể này");


            variant.SkuCode = model.SkuCode;
            variant.BarCode = model.BarCode;
            variant.CostPrice = model.CostPrice;
            variant.SellingPrice = model.SellingPrice;


            if (image != null)
            {
                string imageUrl = await _cloudinaryService.UploadImageAsync(
                    image,
                    "/products/variant/",
                    _cloudinaryService.GenerateImageId(variant.Id.ToString())
                );

                variant.Image = imageUrl;
            }

            ProductVariant updatedProductVariant = _db.ProductVariants.Update(variant).Entity;

            Product product = updatedProductVariant.Product;

            await _elasticsearchService.SaveDocumentById(new
            {
                product.Id,
                VariantId = updatedProductVariant.Id,
                updatedProductVariant.Product.Name,
                Price = updatedProductVariant.SellingPrice,
                updatedProductVariant.SkuCode,
                updatedProductVariant.Image,
                updatedProductVariant.AllowNegativeInventory,
            }, updatedProductVariant.Id.ToString(), "products");

            _db.SaveChanges();
        }

        public List<TopProductViewModel> GetTopProducts(int limit)
        {
            long storeId = _sessionService.GetStoreIdSession();
            var orderItemGroup = _db.OrderItems
                .Where(orderItem => orderItem.Order.StoreId == storeId && orderItem.Type == OrderItemType.PRODUCT)
                .Include(orderItem => orderItem.ProductVariant)
                .ThenInclude(variant => variant.Product)
                .Include(orderItem => orderItem.ProductVariant)
                .ThenInclude(variant => variant.AttributeValues)
                .ThenInclude(attributeValue => attributeValue.AttributeValue)
                .GroupBy(orderItem => orderItem.ProductVariant)
                .Where(group => group.Key != null)
                .Select(group => new
                {
                    ProductName = group.Key.Product.Name,
                    ProductVariant = group.Key,
                    OrderItems = group.ToList()
                }).Take(limit);

            List<TopProductViewModel> topProducts = [];

            foreach (var group in orderItemGroup)
            {
                if (group.ProductVariant == null) continue;
                List<string> variantNames = [];

                foreach (var attribute in group.ProductVariant.AttributeValues)
                {
                    variantNames.Add(attribute.AttributeValue.Value);
                }

                topProducts.Add(new()
                {
                    Id = group.ProductVariant.Id,
                    Name = group.ProductName,
                    VariantName = String.Join("-", variantNames),
                    Revenue = group.OrderItems.Sum(orderItem => orderItem.Total),
                    Image = group.ProductVariant.Image,
                    TotalSale = group.OrderItems.Sum(orderItem => orderItem.Quantity)
                });

            }
            return [.. topProducts.OrderByDescending(product => product.TotalSale)];
        }





        private ProductVariantResponseViewModel ConvertProductVariantToProductVariantResponse(ProductVariant variant)
        {
            return new()
            {
                Id = variant.Id,
                Product = variant.Product,
                BarCode = variant.BarCode,
                CostPrice = variant.CostPrice,
                SellingPrice = variant.SellingPrice,
                CreatedAt = variant.CreatedAt,
                UpdatedAt = variant.UpdatedAt,
                SkuCode = variant.SkuCode,
                Name = String.Join("-", variant.AttributeValues.Select(a => a.AttributeValue.Value)),
                Image = variant.Image,
            };
        }


    }
}
