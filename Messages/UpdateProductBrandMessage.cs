namespace Lemoo_pos.Messages
{
    public class UpdateProductBrandMessage
    {
        public required long ProductId { get; set; }
        public required long BrandId { get; set; }
    }
}