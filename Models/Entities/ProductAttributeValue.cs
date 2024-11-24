using System.ComponentModel.DataAnnotations;

namespace Lemoo_pos.Models.Entities
{
    public class ProductAttributeValue : BaseEntity
    {
       public required ProductAttribute Attribute { get; set; }

        public required string Value  { get; set; }

    }
}