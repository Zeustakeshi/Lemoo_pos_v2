using System.ComponentModel.DataAnnotations;

namespace Lemoo_pos.Models.Entities
{
    public class ProductAttribute : BaseEntity
    {
        public required string Name { get; set; }

    }
}