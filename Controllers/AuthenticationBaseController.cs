using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Lemoo_pos.Controllers
{
    public class AuthenticationBaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            if (HttpContext.Session.Get("AccountId") == null)
            {
                filterContext.Result = new RedirectToActionResult("Login", "Auth", null);
            }
            base.OnActionExecuting(filterContext);
        }
    }
}
