namespace Lemoo_pos.Models.Dto
{
    public class CreateProductDto
    {
        public required string Name { get; set; }

        public required string SkuCode { get; set; }

        public required double SellingPrice {get; set;}

        public required double CostPrice { get; set;}

        public required long Quantity { get; set;}

    }
}
