namespace Lemoo_pos.Models.Dto
{
    public class ProductSearchDto
    {
        public required long Id { get; set; }
        public required long VariantId { get; set; }
        public required long StoreId { get; set; }
        public required List<long> Branches { get; set; }
        public long? Brand { get; set; }
        public long? Category { get; set; }
        public required string Name { get; set; }
        public required string VariantName { get; set; }
        public required double Price { get; set; }
        public required long Quantity { get; set; }
        public string? Image { get; set; }
        public required string SkuCode { get; set; }

        public bool AllowNegativeInventory { get; set; } = false;

        public required string Keyword { get; set; }

    }
}
