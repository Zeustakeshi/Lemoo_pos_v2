using Microsoft.AspNetCore.Mvc;

namespace Lemoo_pos.ViewComponents
{
    public class TopProductViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}