using Lemoo_pos.Areas.Api.Dto;
using Lemoo_pos.Areas.Api.Filters;
using Lemoo_pos.Areas.Api.Services.Interfaces;
using Lemoo_pos.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lemoo_pos.Areas.Api.Controllers
{
    [Route("api/products")]
    [Authorize]
    [TypeFilter(typeof(GlobalExceptionFilter))]
    public class ApiProductController : Controller
    {

        private readonly IProductServiceApi _productServiceApi;

        public ApiProductController(IProductServiceApi productServiceApi)
        {
            _productServiceApi = productServiceApi;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto dto)
        {
            var jsonData = ApiHelper.GetJwtDataDto(User);
            ProductResponseDto response = await _productServiceApi.CreateProduct(
               dto,
               jsonData.AccountId,
               jsonData.StoreId
           );
            Response.StatusCode = 201;
            return Json(response);
        }

        [HttpGet("_count")]
        public IActionResult GetProductCount()
        {
            var jwtData = ApiHelper.GetJwtDataDto(User);
            long productCount = _productServiceApi.GetProductCount(jwtData.StoreId);
            return Json(productCount);
        }
    }
}