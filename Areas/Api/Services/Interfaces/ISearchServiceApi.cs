using Lemoo_pos.Areas.Api.Dto;
using Lemoo_pos.Models.Entities;
using Lemoo_pos.Models.ViewModels;

namespace Lemoo_pos.Areas.Api.Services.Interfaces
{
    public interface ISearchServiceApi
    {
        List<ProductResponseDto> SearchProduct(long storeId, long branchId, string query);
        List<CustomerSearchResponseDto> SearchCustomer(long storeId, string query);
    }
}
