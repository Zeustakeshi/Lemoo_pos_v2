namespace Lemoo_pos.Models.Dto
{
    public class OrderItemDto
    {
        public required long ProductId { get; set; }
        public required long Quantity { get; set; }
        public required long Total { get; set; }
    }
}
