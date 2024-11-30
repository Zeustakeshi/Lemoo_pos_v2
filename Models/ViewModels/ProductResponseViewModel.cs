using Lemoo_pos.Models.Entities;

namespace Lemoo_pos.Models.ViewModels
{
    public class ProductResponseViewModel
    {
        public long Id { get; set; }
        public string? Image { get; set; }
        public required string Name { get; set; }
        public ProductCategory? Category { get; set; }
        public required int VariantCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
