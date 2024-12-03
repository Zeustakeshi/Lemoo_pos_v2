using Lemoo_pos.Models.Dto;

namespace Lemoo_pos.Services.Interfaces
{
    public interface ISearchService
    {
        List<ProductResponseDto> SearchProduct(long storeId, string query); 
    }
}
