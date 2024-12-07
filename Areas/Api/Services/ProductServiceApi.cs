

using Lemoo_pos.Areas.Api.Services.Interfaces;
using Lemoo_pos.Data;
using Lemoo_pos.Helper;
using Lemoo_pos.Areas.Api.Dto;
using Lemoo_pos.Models.Entities;
using Lemoo_pos.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Lemoo_pos.Areas.Api.Services
{
    public class ProductServiceApi : IProductServiceApi
    {
        private readonly ICloudinaryService _cloudinaryService;
        private readonly AppDbContext _db;
        private readonly ISessionService _sessionService;
        private readonly IInventoryService _inventoryService;
        private readonly IElasticsearchService _elasticsearchService;

        public ProductServiceApi(
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


            var productAttribute = new ProductAttribute()
            {
                Name = "Mặc định"
            };

            _db.ProductAttributes.Add(productAttribute);


            var productAttributeValue = new ProductAttributeValue()
            {
                Attribute = productAttribute,
                Value = "Mặc định",
            };

            _db.ProductAttributeValues.Add(productAttributeValue);

            attributeValues.Add((Guid.NewGuid().ToString(), attributeValue: productAttributeValue));

            Branch branch = _db.Branches.Single(b => b.Id == dto.BranchId && b.StoreId == store.Id);

            ProductVariant productVariant = new()
            {
                BarCode = "",
                SellingPrice = Convert.ToInt32(dto.SellingPrice),
                CostPrice = Convert.ToInt32(dto.CostPrice),
                SkuCode = dto.SkuCode,
                Product = product,
                ProductId = product.Id,
                AllowSale = true,
                Image = product.Image,
                AllowNegativeInventory = dto.AllowNegativeInventory
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
                AllowNegativeInventory = dto.AllowNegativeInventory
            }, newProductVariant.Id.ToString(), "products");

            return new()
            {
                Id = product.Id,
                Name = product.Name,
                Price = dto.SellingPrice,
                Quantity = dto.Quantity,
                VariantName = "Mặc định",
                Image = product.Image,
                BranchId = branch.Id,

            };
        }

        public long GetProductCount(long storeId)
        {
            return _db.Products
            .Where(p => p.Store.Id == storeId)
            .Sum(p => p.Variants.Count());
        }



    }
}