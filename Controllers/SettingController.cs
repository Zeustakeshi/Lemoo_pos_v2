using Microsoft.AspNetCore.Mvc;

namespace Lemoo_pos.Controllers
{
    [Route("settings")]
    public class SettingController : AuthenticationBaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
