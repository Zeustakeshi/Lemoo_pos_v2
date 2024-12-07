using Lemoo_pos.Areas.Api.Dto;

namespace Lemoo_pos.Services.Interfaces
{
    public interface ISearchService
    {
        List<ProductResponseDto> SearchProduct(long storeId, long branchId, string query);
        List<CustomerSearchResponseDto> SearchCustomer(long storeId, string query);
    }
}
