using Lemoo_pos.Models.ViewModels;
using Lemoo_pos.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Lemoo_pos.Controllers
{
    [Route("roles")]
    public class RoleController : AuthenticationBaseController
    {

        private readonly IAuthorityService _authorityService;
        public RoleController(IAuthorityService authorityService) {
            _authorityService = authorityService;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return View(_authorityService.GetAllAuthorities());
        }

        [HttpGet("create")]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost("create")]
        public IActionResult SaveRole([FromBody] CreateRoleViewModel model)
        {
            try
            {
                _authorityService.CreateRole(model);
                return Json("Tạo vai trò thành công");
            }catch (Exception ex) { 
                Response.StatusCode = 500;
                return Json(ex.Message);

            }
        }
    }
}
