using Lemoo_pos.Models.Entities;

namespace Lemoo_pos.Models.ViewModels
{
    public class ProductVariantDetailResponseViewModel
    {
        public required long Id { get; set; }

        public required Product Product { get; set; }

        public required string Name { get; set; }

        public required string SkuCode { get; set; }

        public required string BarCode { get; set; }

        public required double SellingPrice { get; set; }

        public required double CostPrice { get; set; }

        public string? Image { get; set; }

        public required DateTime CreatedAt { get; set; }

        public required DateTime UpdatedAt { get; set; }

        public required List<InventoryInfoViewModel> Inventories { get; set; }

        public List<ProductVariantResponseViewModel>? Variants { get; set; } = [];
    }

    
    
}
