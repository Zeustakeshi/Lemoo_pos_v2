namespace Lemoo_pos.Areas.Api.Dto.Response
{
    public class ShiftSaleResponseDto
    {
        public required string PaymentMethod { get; set; }
        public long Revenue { get; set; } = 0;
        public long Expenses { get; set; } = 0;
        public long NetRevenue { get; set; } = 0;
    }
}