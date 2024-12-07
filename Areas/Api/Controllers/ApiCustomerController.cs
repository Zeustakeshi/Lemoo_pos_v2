using Lemoo_pos.Areas.Api.Dto;
using Lemoo_pos.Areas.Api.Filters;
using Lemoo_pos.Areas.Api.Services.Interfaces;
using Lemoo_pos.Helper;
using Lemoo_pos.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lemoo_pos.Areas.Api.Controllers
{
    [Route("api/customers")]
    [Authorize]
    [TypeFilter(typeof(GlobalExceptionFilter))]
    public class ApiCustomerController : Controller
    {
        private readonly ICustomerServiceApi _customerServiceApi;
        public ApiCustomerController(ICustomerServiceApi customerServiceApi)
        {
            _customerServiceApi = customerServiceApi;
        }

        [HttpPost()]
        public IActionResult CreateCustomer([FromBody] CreateCustomerDto dto)
        {
            var jwtData = ApiHelper.GetJwtDataDto(User);
            var response = _customerServiceApi.CreateCustomer(
                   dto,
                   jwtData.StoreId,
                   jwtData.AccountId
               );
            Response.StatusCode = 201;
            return Json(response);
        }

        [HttpGet("_count")]
        public IActionResult GetCustomerCount()
        {
            var jwtData = ApiHelper.GetJwtDataDto(User);
            return Json(_customerServiceApi.GetCustomerCount(jwtData.StoreId));
        }
    }
}