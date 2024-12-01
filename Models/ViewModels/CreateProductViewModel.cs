using Lemoo_pos.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace Lemoo_pos.Models.ViewModels
{
    public class CreateProductViewModel
    {
        [Required(ErrorMessage = "Tên sản phẩm không được bỏ trống.")]
        [StringLength(200, ErrorMessage = "Tên sản phẩm tối đa 200 ký tự.")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Khối lượng sản phẩm không được bỏ trống.")]
        public required double Weight { get; set; }

        [Required(ErrorMessage = "Đơn vị tính không được bỏ trống.")]
        public required string Unit { get; set; }

        [StringLength(5000, ErrorMessage = "Mô tả sản phẩm quá dài")]
        public string? Description { get; set; }

        public long? CategoryId { get; set; }

        public required List<long> Branches { get; set; }

        public  long? BrandId { get; set; }
        public List<ProductVariantViewModel> Variants { get; set; }
        public List<ProductAttributeViewModel> Attributes { get; set; }

    }

    public class ProductVariantViewModel
    {
        public string Name { get; set; }

        public decimal CostPrice { get; set; } = 0;
        public decimal SellingPrice { get; set; } = 0;
        public string SkuCode { get; set; }
        public string BarCode { get; set; }

        public long Quantity { get; set; } = 0;

        public bool AllowSale { get; set; } = false;

        public List<string> AttributeValues { get; set; }
    }

    public class ProductAttributeViewModel
    {
        public string Name { get; set; }
        public List<ProductAttributeValueViewModel> Values { get; set; }
    }

    public class ProductAttributeValueViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }

    }

}
