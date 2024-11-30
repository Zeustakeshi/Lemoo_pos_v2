using Microsoft.AspNetCore.Mvc;

namespace Lemoo_pos.Controllers
{
    [Route("settings")]
    public class SettingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
