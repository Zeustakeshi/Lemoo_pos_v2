using Lemoo_pos.Areas.Api.Filters;
using Lemoo_pos.Areas.Api.Services;
using Lemoo_pos.Areas.Api.Services.Interfaces;
using Lemoo_pos.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lemoo_pos.Areas.Api.Controllers
{

    [Route("api/staffs")]
    [Authorize]
    [TypeFilter(typeof(GlobalExceptionFilter))]
    public class ApiStaffController : Controller
    {
        private readonly IStaffServiceApi _staffServiceApi;
        public ApiStaffController(IStaffServiceApi staffServiceApi)
        {
            _staffServiceApi = staffServiceApi;
        }

        [HttpGet]
        public IActionResult GetAllStaff([FromQuery] long? branchId)
        {
            var jwtData = ApiHelper.GetJwtDataDto(User);
            return Json(_staffServiceApi.GetAllStaff(jwtData.StoreId, branchId));
        }
    }
}