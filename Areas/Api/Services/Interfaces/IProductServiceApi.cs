using Lemoo_pos.Areas.Api.Dto;

namespace Lemoo_pos.Areas.Api.Services.Interfaces
{
    public interface IProductServiceApi
    {
        Task<ProductResponseDto> CreateProduct(CreateProductDto dto, long accountId, long storeId);
        long GetProductCount(long storeId);
    }
}
