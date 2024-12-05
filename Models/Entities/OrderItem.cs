using System.ComponentModel.DataAnnotations;
using Lemoo_pos.Common.Enums;

namespace Lemoo_pos.Models.Entities
{
    public class OrderItem : BaseEntity
    {
        public required Order Order { get; set; }
        public required long OrderId { get; set; }
        public ProductVariant? ProductVariant { get; set; }
        public long? ProductVariantId { get; set; }
        public string? ServiceName { get; set; }
        public OrderItemType Type { get; set; } = OrderItemType.PRODUCT;
        public required long Quantity { get; set; }
        public required long Total { get; set; }
    }
}