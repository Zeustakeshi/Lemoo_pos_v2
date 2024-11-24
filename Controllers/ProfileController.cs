using Microsoft.AspNetCore.Mvc;

namespace Lemoo_pos.Controllers
{
    public class ProfileController : Controller
    {

        [HttpGet]
        [Route("profile")]
        public IActionResult Index()
        {
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.Email = HttpContext.Session.GetString("Email");
            ViewBag.Avatar = HttpContext.Session.GetString("Avatar");
            return View();
        }
    }
}
