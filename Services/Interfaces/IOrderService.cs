using Lemoo_pos.Areas.Api.Dto;
using Lemoo_pos.Models.ViewModels;

namespace Lemoo_pos.Services.Interfaces
{
    public interface IOrderService
    {
        List<PaymentMethodAnaliticsViewModel> GetPaymentMethodAnalytics();
        List<SalesOverviewViewModel> GetSalesOverview();
    }
}
