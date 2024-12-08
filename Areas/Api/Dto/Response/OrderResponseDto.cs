namespace Lemoo_pos.Areas.Api.Dto
{
    public class OrderResponseDto
    {
        public required long Id { get; set; }
        public required List<OrderItemDto> Items { get; set; }
        public long? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public required long Total { get; set; }
        public required string PaymentMethod { get; set; }
        public required DateTime CreatedAt { get; set; }
        public required DateTime UpdatedAt { get; set; }
    }
}
