
using Lemoo_pos.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Lemoo_pos.ViewComponents
{
    public class PaymentMethodChartViewComponent : ViewComponent
    {
        private readonly IOrderService _orderService;
        public PaymentMethodChartViewComponent(IOrderService orderService)
        {
            _orderService = orderService;
        }
        public IViewComponentResult Invoke(bool allowMultiSelect)
        {
            return View(_orderService.GetPaymentMethodAnalytics());
        }

    }

}