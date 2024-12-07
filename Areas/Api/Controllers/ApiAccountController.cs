
using Lemoo_pos.Areas.Api.Filters;
using Lemoo_pos.Areas.Api.Services.Interfaces;
using Lemoo_pos.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lemoo_pos.Areas.Api.Controllers
{
    [Route("api/accounts")]
    [Authorize]
    [TypeFilter(typeof(GlobalExceptionFilter))]
    public class ApiAccountController : Controller
    {
        private readonly IAccountServiceApi _accountServiceApi;
        public ApiAccountController(IAccountServiceApi accountServiceApi)
        {
            _accountServiceApi = accountServiceApi;
        }


        [HttpGet("me")]
        public IActionResult GetAccountInfo()
        {
            var jwtData = ApiHelper.GetJwtDataDto(User);
            return Json(_accountServiceApi.GetAccountById(jwtData.AccountId));
        }

    }
}