using Lemoo_pos.Models.ViewModels;
using Lemoo_pos.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Lemoo_pos.Controllers
{
    [Route("staffs")]
    public class StaffController : AuthenticationBaseController
    {

        private readonly IStaffService _staffService;
        private readonly ISessionService _sesstionService;
        public StaffController(IStaffService staffService, ISessionService sessionService)
        {
            _sesstionService = sessionService;
            _staffService = staffService;
        }

        public IActionResult Index()
        {
            long storeId = _sesstionService.GetStoreIdSession();
            return View(_staffService.GetAllStaff(storeId));
        }

        [HttpGet("create")]
        public IActionResult CreateStaff()
        {
            return View(_staffService.GetAllStaffStatus());
        }


        [HttpPost("create")]
        public IActionResult CreateStaff([FromBody] CreateStaffViewModel model)
        {
            try
            {

                Response.StatusCode = 201;
                _staffService.CreateStaff(model);
                return Json("Thêm nhân viên thành công");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Response.StatusCode = 500;
                return Json("Tạo nhân viên thất bại");
            }
        }
    }
}
