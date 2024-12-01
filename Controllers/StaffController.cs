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
            return View();
        }
    }
}
