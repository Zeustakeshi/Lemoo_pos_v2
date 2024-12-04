namespace Lemoo_pos.Models.Dto
{
    public class ProductResponseDto
    {
        public required long Id { get; set; }
        public required string Name { get; set; }
        public required long BranchId { get; set; }
        public required string VariantName { get; set; }
        public required double Price { get; set; }
        public required long Quantity { get; set; }
        public string? Image { get; set; }
        public bool AllowNegativeInventory { get; set; } = false;

    }
}
