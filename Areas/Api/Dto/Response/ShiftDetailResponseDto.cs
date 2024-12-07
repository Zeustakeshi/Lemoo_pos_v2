using Lemoo_pos.Areas.Api.Dto.Response;

namespace Lemoo_pos.Areas.Api.Dto
{
    public class ShiftDetailResponseDto : ShiftResponseDto
    {
        public List<ShiftOrderResponseDto> Orders { get; set; } = [];
    }
}