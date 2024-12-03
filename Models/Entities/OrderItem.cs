using System.ComponentModel.DataAnnotations;

namespace Lemoo_pos.Models.Entities
{
    public class OrderItem : BaseEntity
    {
        public required Order Order { get; set; }

        public required long OrderId { get; set; }

        public required ProductVariant ProductVariant { get; set; }

        public required long ProductVariantId { get; set; }

        public required long Quantity { get; set; }

        public required long Total { get; set; }
    }
}