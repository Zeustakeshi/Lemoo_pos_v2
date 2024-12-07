using Lemoo_pos.Common.Enums;

namespace Lemoo_pos.Areas.Api.Dto.Response
{
    public class ShiftOrderResponseDto
    {
        public required long Id { get; set; }
        public required string PaymentMethod { get; set; }
        public required long Total { get; set; }
        public required DateTime CreatedAt;
    }
}