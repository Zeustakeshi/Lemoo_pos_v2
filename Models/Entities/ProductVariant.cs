using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Lemoo_pos.Common.Enums;

namespace Lemoo_pos.Models.Entities
{
    public class ProductVariant : BaseEntity
    {
        public required Product Product { get; set; }
        public required long ProductId { get; set; }

        public required double CostPrice { get; set; } = 0;

        public required double SellingPrice { get; set; } = 0;

        public bool AllowSale { get; set; } = true;

        public required string SkuCode { get; set; }

        public required string BarCode { get; set; }

        public string? Image { get; set; }

        public bool AllowNegativeInventory { get; set; } = false;

        public List<Inventory> Inventories { get; set; } = [];

        public List<ProductVariantAttribute> AttributeValues { get; set; } = [];
        public SaveStatus SaveStatus { get; set; }
    }
}