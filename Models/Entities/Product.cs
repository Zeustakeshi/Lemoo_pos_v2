using System.ComponentModel.DataAnnotations;

namespace Lemoo_pos.Models.Entities
{
    public class Product : BaseEntity
    {
        public required string Name { get; set; }

        public string? Unit { get; set; }

        public string? Description { get; set; }

    }
}