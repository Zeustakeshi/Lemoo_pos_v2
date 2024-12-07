using Lemoo_pos.Areas.Api.Filters;
using Lemoo_pos.Areas.Api.Services.Interfaces;
using Lemoo_pos.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Lemoo_pos.Helper;


namespace Lemoo_pos.Areas.Api.Controllers
{
    [Route("api/pos")]
    [Authorize]
    [TypeFilter(typeof(GlobalExceptionFilter))]
    public class ApiPosController() : Controller
    {


    }
}
