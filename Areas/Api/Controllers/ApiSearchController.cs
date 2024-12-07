using Lemoo_pos.Areas.Api.Filters;
using Lemoo_pos.Areas.Api.Services.Interfaces;
using Lemoo_pos.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lemoo_pos.Areas.Api.Controllers
{
    [Route("api/search")]
    [Authorize]
    [TypeFilter(typeof(GlobalExceptionFilter))]
    public class ApiSearchController : Controller
    {
        private readonly ISearchServiceApi _searchServiceApi;
        public ApiSearchController(ISearchServiceApi searchServiceApi)
        {
            _searchServiceApi = searchServiceApi;
        }

        [HttpGet("products")]
        public IActionResult SearchProduct([FromQuery] string query, [FromQuery] long branchId)
        {
            var jwtData = ApiHelper.GetJwtDataDto(User);
            return Json(_searchServiceApi.SearchProduct(jwtData.StoreId, branchId, query));
        }

        [HttpGet("customers")]
        public IActionResult SearchCustomer([FromQuery] string query)
        {
            var jwtData = ApiHelper.GetJwtDataDto(User);
            return Json(_searchServiceApi.SearchCustomer(jwtData.StoreId, query));
        }
    }
}