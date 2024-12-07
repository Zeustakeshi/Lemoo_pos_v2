using Lemoo_pos.Common.Enums;

namespace Lemoo_pos.Areas.Api.Dto
{
    public class OrderItemDto
    {
        public long? ProductId { get; set; }
        public required long Quantity { get; set; }
        public required long Total { get; set; }
        public string? ServiceName { get; set; }
        public OrderItemType Type { get; set; } = OrderItemType.SERVICE;
    }
}
