using Microsoft.AspNetCore.Mvc;

namespace Lemoo_pos.ViewComponents
{
    public class StoreOverviewViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}