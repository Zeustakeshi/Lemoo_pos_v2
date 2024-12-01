using System.ComponentModel.DataAnnotations;

namespace Lemoo_pos.Models.Entities
{
    public class InventoryLog : BaseEntity
    {
        public required Inventory Inventory { get; set; }

        public required long InventoryId { get; set; }

        public required Staff Staff { get; set; }

        public required long StaffId { get; set; }

        public required string Action { get; set; }

        public long? PreviousQuantity { get; set; }
        public long? NewQuantity { get; set; }

        public long? PreviousAvailableQuantity { get; set; }
        public long? NewAvailableQuantity { get; set; }


    }
}