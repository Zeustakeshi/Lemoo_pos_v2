using Lemoo_pos.Data;
using Lemoo_pos.Services;
using Lemoo_pos.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Lemoo_pos.ViewComponents
{
    public class SalesOverviewViewComponent : ViewComponent
    {

        private readonly IOrderService _orderService;
        public SalesOverviewViewComponent(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public IViewComponentResult Invoke()
        {
            return View(_orderService.GetSalesOverview());
        }
    }
}