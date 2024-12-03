namespace Lemoo_pos.Models.Dto
{
    public class CreateOrderDto
    {
        public required  List<OrderItemDto> Items { get; set; }
        
        public long? CustomerId { get; set; }

        public required long Total { get; set; }

        public required long Change { get; set; }

        public required string PaymentMethod { get; set; }

    }

}
