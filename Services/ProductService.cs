using CloudinaryDotNet;
using Lemoo_pos.Data;
using Lemoo_pos.Models.Entities;
using Lemoo_pos.Models.ViewModels;
using Lemoo_pos.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;

namespace Lemoo_pos.Services
{
    public class ProductService : IProductService
    {
        private readonly ICloudinaryService _cloudinaryService;
        private readonly AppDbContext _db;
        private readonly HttpContext _httpContext;

        public ProductService (ICloudinaryService cloudinaryService, AppDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _cloudinaryService = cloudinaryService;
            _db = db;
            _httpContext = httpContextAccessor.HttpContext;
        }


        public List<ProductResponseViewModel> GetAllProduct()
        {
            List<Product> products = [.. _db.Products
                .Include(p => p.Variants)
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
                    CreatedAt = product.CreatedAt,
                    UpdatedAt = product.UpdatedAt
                });
            }

            return productResponses;
        }

        public async Task CreateProduct(CreateProductViewModel productData, IFormFile image)
        {
            long storeId = Convert.ToInt64(_httpContext.Session.GetString("StoreId"));

            Store store = _db.Stores.Single(s => s.Id.Equals(storeId));

            if (store == null) throw new Exception("Store not found. ");

        
            Product product = new()
            {
                Name = productData.Name,
                Unit = productData.Unit,
                Description = productData.Description,
                Store = store,
            };


          
            if (image != null)
            {
                string imageUrl = await _cloudinaryService.UploadImageAsync(image, "/products");
                product.Image = imageUrl;
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
                    Quantity = variant.Quantity,
                    AllowSale = variant.AllowSale,
                    Image = product.Image
                };

                _db.ProductVariants.Add(productVariant);

                foreach (var attributeId in variant.AttributeValues)
                {
                    ProductAttributeValue value = attributeValues.FirstOrDefault(a => a.Id.Equals(attributeId)).attributeValue;
                    if (value == null) continue;

                    _db.ProductVariantAttributes.Add(new() { 
                        Variant = productVariant,
                        AttributeValue = value
                    });
                }

            }
        
            _db.SaveChanges();

                
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
            var variant = _db.ProductVariants
                .Include(v => v.AttributeValues)
                .ThenInclude(av => av.AttributeValue)
                .Include(v => v.Product)
                .Where(v => v.Id == variantId && v.ProductId == productId).Single();

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
                Variants = GetAllVariants(productId).Where(v => v.Id != variantId).ToList()
            };

            return response;
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
