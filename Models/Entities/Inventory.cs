using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lemoo_pos.Models.Entities
{
    public class Inventory : BaseEntity
    {
        
        public required Branch Branch { get; set; }

        public required ProductVariant ProductVariant { get; set; }

        public required long ProductVariantId { get; set; }

        public required long Quantity { get; set; } = 0;

        public required long Available { get; set; } = 0;

    }
}