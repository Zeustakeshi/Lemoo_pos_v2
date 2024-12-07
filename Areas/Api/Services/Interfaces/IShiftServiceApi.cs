using Lemoo_pos.Areas.Api.Dto;
using Lemoo_pos.Areas.Api.Dto.Response;

namespace Lemoo_pos.Areas.Api.Services.Interfaces
{
    public interface IShiftServiceApi
    {
        List<ShiftResponseDto> GetAllShift(long storeId, long? staffId, DateTime? startTime, DateTime? endTime);
        long CreateShift(long storeId, long accountId, ShiftRequestDto dto);
        ShiftDetailResponseDto CloseShift(long storeId, long accountId, ShiftRequestDto dto);
        List<ShiftOrderResponseDto> GetAllShiftOrderByShiftId(long storeId, long shiftId);
        ShiftDetailResponseDto GetShiftById(long storeId, long shiftId);
    }
}