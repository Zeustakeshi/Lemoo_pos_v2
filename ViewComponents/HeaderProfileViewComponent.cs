using Microsoft.AspNetCore.Mvc;

namespace Lemoo_pos.ViewComponents
{
    public class HeaderProfileViewComponent : ViewComponent
    {

        public IViewComponentResult Invoke()
        {
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.Email = HttpContext.Session.GetString("Email");
            ViewBag.Avatar = HttpContext.Session.GetString("Avatar");

            return View();
        }
    }
}