namespace Lemoo_pos.Models.ViewModels
{
    public class InventoryInfoViewModel
    {
        public required long Id { get; set; }
        public required string BranchName { get; set; }
        public required long Quantity {  get; set; }
        public required long Available { get; set; }

    }
}
