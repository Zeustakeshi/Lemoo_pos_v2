using System.ComponentModel.DataAnnotations;

namespace Lemoo_pos.Models.Entities
{
    public class Brand : BaseEntity
    {
        public required string Name { get; set; }

        public required Store Store { get; set; }

        public required long StoreId { get; set; }

        public List<Product> Products { get; set; } = [];
    }
}