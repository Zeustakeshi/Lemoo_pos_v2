using Lemoo_pos.Models.Entities;

namespace Lemoo_pos.Models.ViewModels
{
    public class InventoryHistoryViewModel
    {
        public required long PreviousQuantity { get; set; }
        public required long NewQuantity { get; set; }
        public required string BranchName { get; set; }
        public required long PreviousAvailableQuantity { get; set; }
        public required long NewAvailableQuantity { get; set; }
        public required Staff UpdateBy { get; set; }
        public required string Action { get; set; }
        public required DateTime CreatedAt { get; set; }
        public required DateTime UpdatedAt { get; set; }
    }
}
