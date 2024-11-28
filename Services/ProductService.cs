using CloudinaryDotNet;
using Lemoo_pos.Data;
using Lemoo_pos.Models.Entities;
using Lemoo_pos.Models.ViewModels;
using Lemoo_pos.Services.Interfaces;
using System;

namespace Lemoo_pos.Services
{
    public class ProductService : IProductService
    {
        private readonly Cloudinary _cloudinary;
        private readonly AppDbContext _db;

        public ProductService (Cloudinary cloudinary, AppDbContext db)
        {
            _cloudinary = cloudinary;
            _db = db;
        }

        public CreateProductViewModel CreateProduct(CreateProductViewModel productData)
        {
            Product product = new()
            {
                Name = productData.Name,
                Unit = productData.Unit,
                Description = productData.Description
            };

            _db.Products.Add(product);


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
                    Product = product
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


            return productData;

        }
    }
}
