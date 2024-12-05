using Lemoo_pos.Models.Dto;
using Lemoo_pos.Models.Entities;
using Lemoo_pos.Models.ViewModels;

namespace Lemoo_pos.Services.Interfaces
{
    public interface IProductService
    {
        Task CreateProduct(CreateProductViewModel model, IFormFile image);

        Task<ProductResponseDto> CreateProduct(CreateProductDto dto, long accountId, long storeId);

        List<ProductResponseViewModel> GetAllProduct();

        List<ProductVariantResponseViewModel> GetAllVariants(long productId);

        ProductVariantDetailResponseViewModel GetProductVariantByIdAndProductID(long variantId, long productId);

        Task UpdateProductVariant(long productId, long variantId, UpdateProductVariantViewModel model, IFormFile image);

        void DeleteProduct(long productId);

        long GetProductCount(long storeId);

    }
}
