using System.ComponentModel.DataAnnotations;

namespace Lemoo_pos.Models.Entities
{
    public class ProductCategory : BaseEntity
    {
        public required string Name { get; set; }

        public string? Description { get; set; }

        public required Store Store { get; set; }

        public List<Product> Products { get; set; } = [];

    }
}