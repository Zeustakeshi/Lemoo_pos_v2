using Lemoo_pos.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Lemoo_pos.ViewComponents
{
    public class StoreOverviewViewComponent : ViewComponent
    {
        private readonly IStoreService _storeService;
        public StoreOverviewViewComponent(IStoreService storeService)
        {
            _storeService = storeService;
        }
        public IViewComponentResult Invoke()
        {
            return View(_storeService.GetStoreOverview());
        }
    }
}