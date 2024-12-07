using System.ComponentModel.DataAnnotations;

namespace Lemoo_pos.Models.Entities
{
    public class Store : BaseEntity
    {
        public required string Name { get; set; }
        // public bool AllowManageShift { get; set; } = false;

    }
}