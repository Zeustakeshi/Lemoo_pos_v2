namespace Lemoo_pos.Models.ViewModels
{
    public class UpdateVariantInventoryViewModel
    {
        public required string Reason { get; set; }

        public required long Available { get; set; }

        public required long Quantity { get; set; }
    }
}
