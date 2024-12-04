using CloudinaryDotNet;
using Lemoo_pos.Data;
using Lemoo_pos.Models.Dto;
using Lemoo_pos.Models.Entities;
using Lemoo_pos.Models.ViewModels;
using Lemoo_pos.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;
using Nest;
using System.Text.RegularExpressions;
using Lemoo_pos.Helper;

namespace Lemoo_pos.Services
{
    public class ProductService : IProductService
    {
        private readonly ICloudinaryService _cloudinaryService;
        private readonly AppDbContext _db;
        private readonly ISessionService _sessionService;
        private readonly IInventoryService _inventoryService;
        private readonly IElasticsearchService _elasticsearchService;

        public ProductService (
            ICloudinaryService cloudinaryService, 
            AppDbContext db, 
            ISessionService sessionService,
            IInventoryService inventoryService,
            IElasticsearchService elasticsearchService
        )
        {
            _cloudinaryService = cloudinaryService;
            _db = db;
            _inventoryService = inventoryService;
            _sessionService = sessionService;
            _elasticsearchService = elasticsearchService;
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

            Store store = _sessionService.GetStoreSession();
            Staff staff = _sessionService.GetStaffSession();

            Product product = new()
            {
                Name = productData.Name,
                Unit = productData.Unit,
                Description = productData.Description,
                Store = store,
            };

            if (image != null)
            {
                string imageUrl = await _cloudinaryService.UploadImageAsync(image, "/products", _cloudinaryService.GenerateImageId(product.Id.ToString()));
                product.Image = imageUrl;
            }else
            {
                product.Image = "http://res.cloudinary.com/dymmvrufy/image/upload/v1732958992/lemoo_pos/products/cgvy4owbou09tb6okwvu.svg";
            }

            try
            {
             _db.Products.Add(product);

            }catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                throw; 
            }
                
            if (productData.CategoryId != null)
            {
                ProductCategory category =  _db.ProductCategories.Single(category => category.Id == productData.CategoryId);
                if (category != null)
                {
                    product.Category = category;
                    product.CategoryId = category.Id;
                }
            }

            if (productData.BrandId != null)
            {
                Brand brand = _db.Brands.Single(brand => brand.Id == productData.BrandId);
                if (brand != null)
                {
                    product.Brand = brand;
                    product.BrandId = brand.Id;
                }
            }

            var attributeValues = new List<(string Id, ProductAttributeValue attributeValue)>();


            foreach (var attribute in productData.Attributes)
            {
                var procductAttribute = new ProductAttribute()
                {
                    Name = attribute.Name
                };

                _db.ProductAttributes.Add(procductAttribute);


                foreach (var value in attribute.Values)
                {
                    var productAttributeValue = new ProductAttributeValue()
                    {
                        Attribute = procductAttribute,
                        Value = value.Name,
                    };

                    _db.ProductAttributeValues.Add(productAttributeValue);

                    attributeValues.Add((value.Id, attributeValue: productAttributeValue));
                }
            }

            List<Branch> branches = [.. _db.Branches.Where(branch => productData.Branches.Contains(branch.Id))];

            foreach (var variant in productData.Variants)
            {
                ProductVariant productVariant = new()
                {
                    BarCode = variant.BarCode,
                    SellingPrice = Convert.ToInt32(variant.SellingPrice),
                    CostPrice = Convert.ToInt32(variant.CostPrice),
                    SkuCode = variant.SkuCode,
                    Product = product,
                    ProductId = product.Id,
                    AllowSale = variant.AllowSale,
                    Image = product.Image
                };

                var newProductVariant = _db.ProductVariants.Add(productVariant).Entity;

                foreach (var branch in branches)
                {
                   

                    try
                    {
                       _inventoryService.CreateInventory(
                        newProductVariant,
                        branch,
                        variant.Quantity,
                        variant.Quantity,
                        staff: staff
                       );
                    }
                    catch (Exception ex)
                    {
                        await Console.Out.WriteLineAsync(ex.Message);
                        throw new Exception("Lưu kho thất bại");
                    }

                }

                List<string> variantNames = [];

                foreach (var attributeId in variant.AttributeValues)
                {
                    ProductAttributeValue value = attributeValues.FirstOrDefault(a => a.Id.Equals(attributeId)).attributeValue;
                    if (value == null) continue;
                    
                    _db.ProductVariantAttributes.Add(new() { 
                        Variant = productVariant,
                        AttributeValue = value
                    });

                    variantNames.Add(value.Value);
                }

                await _elasticsearchService.SaveDocumentById(new ProductSearchDto()
                {
                    Id = product.Id,
                    Branches = [..productData.Branches],
                    Name = product.Name,
                    Price = variant.SellingPrice,
                    Quantity = variant.Quantity,
                    SkuCode = variant.SkuCode,
                    StoreId = store.Id,
                    VariantName = String.Join("-", variantNames),
                    Image = product.Image,
                    VariantId = newProductVariant.Id,
                    Keyword = LanguageHelper.RemoveVietnameseTones(product.Name + " " + String.Join("-", variantNames)),
                }, newProductVariant.Id.ToString(), "products");

            }
        
            _db.SaveChanges();

        }

        public async Task<ProductResponseDto> CreateProduct(CreateProductDto dto, long accountId, long storeId)
        {

            Store store = _db.Stores.Single(s => s.Id.Equals(storeId)) ?? throw new Exception("Store not found");
            Staff staff = _db.Staffs
                .Include(s => s.Account)
                .Single(s => s.Account.Id.Equals(accountId));

            Product product = new()
            {
                Name = dto.Name,
                Store = store,
                Image = "http://res.cloudinary.com/dymmvrufy/image/upload/v1732958992/lemoo_pos/products/cgvy4owbou09tb6okwvu.svg"
            };

            _db.Products.Add(product);


            var attributeValues = new List<(string Id, ProductAttributeValue attributeValue)>();


            var procductAttribute = new ProductAttribute()
            {
                Name = "Mặc định"
            };

            _db.ProductAttributes.Add(procductAttribute);


            var productAttributeValue = new ProductAttributeValue()
            {
                Attribute = procductAttribute,
                Value = "Mặc định",
            };

            _db.ProductAttributeValues.Add(productAttributeValue);

            attributeValues.Add((Guid.NewGuid().ToString(), attributeValue: productAttributeValue));

            Branch branch = _db.Branches.Single(b => (b.IsDefaultBranch ?? false) && b.StoreId.Equals(storeId));

            ProductVariant productVariant = new()
            {
                BarCode = "",
                SellingPrice = Convert.ToInt32(dto.SellingPrice),
                CostPrice = Convert.ToInt32(dto.CostPrice),
                SkuCode = dto.SkuCode,
                Product = product,
                ProductId = product.Id,
                AllowSale = true,
                Image = product.Image
            };

            var newProductVariant = _db.ProductVariants.Add(productVariant).Entity;

            try
            {
                _inventoryService.CreateInventory(
                     newProductVariant,
                     branch,
                     dto.Quantity,
                     dto.Quantity,
                     staff: staff
                 );
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                throw new Exception("Lưu kho thất bại");
            }

            if (attributeValues.Count > 0)
            {
                ProductAttributeValue value = attributeValues[0].attributeValue;

                _db.ProductVariantAttributes.Add(new()
                {
                    Variant = productVariant,
                    AttributeValue = value
                });
            }

            _db.SaveChanges();


            await _elasticsearchService.SaveDocumentById(new ProductSearchDto()
            {
                Id = product.Id,
                VariantId = newProductVariant.Id,
                Branches = [branch.Id],
                Name = product.Name,
                Price = dto.SellingPrice,
                Quantity = dto.Quantity,
                SkuCode = dto.SkuCode,
                StoreId = store.Id,
                VariantName = "Mặc định",
                Image = product.Image,
                Keyword = LanguageHelper.RemoveVietnameseTones(product.Name + " " + "Mặc định"),
            }, newProductVariant.Id.ToString(), "products");


            return new()
            {
                Id = product.Id,
                Name = product.Name,
                Price = dto.SellingPrice,
                Quantity = dto.Quantity,
                VariantName = "Mặc định",
                Image = product.Image

            };
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
                        Available = inventory.Available ,
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

        public void DeleteProduct (long productId) 
        {
            Product product = _db.Products.Single(p => p.Id ==  productId);
            if (product == null)
            {
                throw new Exception("Không tìm thấy sản phẩm này");
            }


            product.IsDeleted = true;

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
            },  updatedProductVariant.Id.ToString(), "products");

            _db.SaveChanges();
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
