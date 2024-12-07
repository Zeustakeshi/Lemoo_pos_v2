
using Lemoo_pos.Common.Enums;

namespace Lemoo_pos.Models.Entities
{
    public class Product : BaseEntity
    {
        public required string Name { get; set; }
        public string? Image { get; set; }
        public string? Unit { get; set; }
        public string? Description { get; set; }
        public Brand? Brand { get; set; }
        public long? BrandId { get; set; }
        public ProductCategory? Category { get; set; }
        public long? CategoryId { get; set; }
        public bool? IsDeleted { get; set; } = false;
        public required Store Store { get; set; }

        public List<ProductVariant> Variants { get; set; } = [];
        public SaveStatus SaveStatus { get; set; } = SaveStatus.PENDING;
    }
}