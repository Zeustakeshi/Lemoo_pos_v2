using CloudinaryDotNet;
using Lemoo_pos.Areas.Api.Dto;
using Lemoo_pos.Areas.Api.Filters;
using Lemoo_pos.Areas.Api.Services.Interfaces;
using Lemoo_pos.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nest;

namespace Lemoo_pos.Controllers
{
    [Route("api/shifts")]
    [Authorize]
    [TypeFilter(typeof(GlobalExceptionFilter))]
    public class ApiShiftController : Controller
    {
        private readonly IShiftServiceApi _shiftServiceApi;
        public ApiShiftController(IShiftServiceApi shiftServiceApi)
        {
            _shiftServiceApi = shiftServiceApi;
        }

        [HttpGet]
        public IActionResult GetAllShift(
            [FromQuery] long? staffId,
            [FromQuery] DateTime? startTime,
            [FromQuery] DateTime? endTime
        )
        {
            var jwtData = ApiHelper.GetJwtDataDto(User);
            return Json(_shiftServiceApi.GetAllShift(
                    storeId: jwtData.StoreId,
                    staffId,
                    startTime,
                    endTime
                ));
        }

        [HttpGet("{shiftId}")]
        public IActionResult GetShiftById(long shiftId)
        {
            var jwtData = ApiHelper.GetJwtDataDto(User);
            return Json(_shiftServiceApi.GetShiftById(jwtData.StoreId, shiftId));
        }

        [HttpPost("open")]
        public IActionResult CreateShift([FromBody] ShiftRequestDto dto)
        {
            var jwtData = ApiHelper.GetJwtDataDto(User);
            Response.StatusCode = 201;
            return Json(_shiftServiceApi.CreateShift(jwtData.StoreId, jwtData.AccountId, dto));
        }

        [HttpPost("close")]
        public IActionResult CloseShift([FromBody] ShiftRequestDto dto)
        {
            var jwtData = ApiHelper.GetJwtDataDto(User);
            return Json(_shiftServiceApi.CloseShift(jwtData.StoreId, jwtData.AccountId, dto));
        }

        [HttpGet("{shiftId}/orders")]
        public IActionResult GetAllOrderByShiftId(long shiftId)
        {
            var jwtData = ApiHelper.GetJwtDataDto(User);
            return Json(_shiftServiceApi.GetAllShiftOrderByShiftId(jwtData.StoreId, shiftId));
        }

    }
}
