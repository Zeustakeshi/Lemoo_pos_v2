namespace Lemoo_pos.Models.ViewModels
{
    public class UpdateProductVariantViewModel
    {
        public required string SkuCode { get; set; }
        public required string BarCode { get; set; }

        public required double CostPrice { get; set; }

        public required double SellingPrice { get; set; }
    }
}
