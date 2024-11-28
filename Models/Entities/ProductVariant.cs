using System.ComponentModel.DataAnnotations;

namespace Lemoo_pos.Models.Entities
{
    public class ProductVariant : BaseEntity
    {
        public required Product Product { get; set; }

        public required double CostPrice { get; set; }

        public required double SellingPrice { get; set; }

        public required string SkuCode { get; set; }

        public required string BarCode { get; set; }

        public string? Image { get; set; }

    }
}