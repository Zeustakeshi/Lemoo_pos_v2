using Lemoo_pos.Areas.Api.Filters;
using Lemoo_pos.Areas.Api.Services.Interfaces;
using Lemoo_pos.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lemoo_pos.Areas.Api.Controllers
{
    [Route("api/branches")]
    [Authorize]
    [TypeFilter(typeof(GlobalExceptionFilter))]
    public class ApiBranchController : Controller
    {
        private readonly IBranchServiceApi _branchServiceApi;
        public ApiBranchController(IBranchServiceApi branchServiceApi)
        {
            _branchServiceApi = branchServiceApi;
        }

        [HttpGet]
        public IActionResult GetAllBranch()
        {
            var jwtData = ApiHelper.GetJwtDataDto(User);
            return Json(_branchServiceApi.GetAllBranchByStoreId(jwtData.StoreId));
        }
    }
}