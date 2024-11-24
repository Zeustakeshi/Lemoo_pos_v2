using System.ComponentModel.DataAnnotations;

namespace Lemoo_pos.Models.Entities
{
    public class ProductVariantAttribute : BaseEntity
    {
       public required ProductVariant Variant { get; set; }
    
        public required ProductAttributeValue AttributeValue { get; set; }
    }
}