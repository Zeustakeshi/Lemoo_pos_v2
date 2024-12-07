using Lemoo_pos.Areas.Api.Dto;
using Lemoo_pos.Areas.Api.Filters;
using Lemoo_pos.Areas.Api.Services.Interfaces;
using Lemoo_pos.Helper;
using Lemoo_pos.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nest;

namespace Lemoo_pos.Areas.Api.Controllers
{

    [Route("api/orders")]
    [Authorize]
    [TypeFilter(typeof(GlobalExceptionFilter))]
    public class ApiOrderController : Controller
    {
        private readonly IOrderServiceApi _orderServiceApi;
        public ApiOrderController(IOrderServiceApi orderServiceApi)
        {
            _orderServiceApi = orderServiceApi;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
        {
            var jwtData = ApiHelper.GetJwtDataDto(User);
            OrderResponseDto response = await _orderServiceApi.CreateOrder(
                dto,
                jwtData.StoreId,
                jwtData.AccountId
            );
            Response.StatusCode = 201;
            return Json(response);
        }


        [HttpPost("batch")]
        public IActionResult CreateOrderBatch([FromBody] List<CreateOrderDto> dto)
        {
            var jwtData = ApiHelper.GetJwtDataDto(User);
            _orderServiceApi.CreateOrderBatch(
               dto,
               jwtData.StoreId,
               jwtData.AccountId
           );
            Response.StatusCode = 201;
            return Json("Sync order sucess");
        }

    }
}