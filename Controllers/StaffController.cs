using Lemoo_pos.Models.ViewModels;
using Lemoo_pos.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Lemoo_pos.Controllers
{
    [Route("staffs")]
    public class StaffController : AuthenticationBaseController
    {

        private readonly IStaffService _staffService;
        public StaffController(IStaffService staffService)
        {
            _staffService = staffService;
        }


        public IActionResult Index()
        {
            return View(_staffService.GetAllStaff());
        }

        [HttpGet("create")]
        public IActionResult CreateStaff ()
        {
            return View(_staffService.GetAllStaffStatus());
        }


        [HttpPost("create")]
        public IActionResult CreateStaff ([FromBody] CreateStaffViewModel model)
        {
            try
            {

                Response.StatusCode = 201;
                _staffService.CreateStaff(model);
                return Json("Thêm nhân viên thành công");

            }catch (Exception ex) {
                Console.WriteLine(ex.Message);
                Response.StatusCode = 500;
                return Json("Tạo nhân viên thất bại");
            }
        }
    }
}
