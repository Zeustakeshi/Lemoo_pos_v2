namespace Lemoo_pos.Messages
{
    public class UpdateOrderCustomerMessage
    {
        public required long OrderId { get; set; }
        public required long CustomerId { get; set; }
    }
}