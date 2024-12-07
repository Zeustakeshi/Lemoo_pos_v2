namespace Lemoo_pos.Models.ViewModels
{
    public class TopProductViewModel
    {
        public required long Id { get; set; }
        public string? Image { get; set; }
        public required long TotalSale { get; set; }
        public required string Name { get; set; }
        public required string VariantName { get; set; }
        public long Revenue { get; set; } = 0;
    }
}