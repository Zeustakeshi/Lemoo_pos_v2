using System.ComponentModel.DataAnnotations;

namespace Lemoo_pos.Models.Entities
{
    public class ProductVariant : BaseEntity
    {
        public required Product Product { get; set; }

        public required double Price { get; set; }

        public required string SkuCode { get; set; }

        public required string BarCode { get; set; }

        public string? Image { get; set; }

    }
}