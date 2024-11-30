using Lemoo_pos.Models.Entities;
using Lemoo_pos.Models.ViewModels;

namespace Lemoo_pos.Services.Interfaces
{
    public interface IProductService
    {
        Task CreateProduct(CreateProductViewModel model, IFormFile image);

        List<ProductResponseViewModel> GetAllProduct();

        List<ProductVariantResponseViewModel> GetAllVariants(long productId);

        ProductVariantDetailResponseViewModel GetProductVariantByIdAndProductID(long variantId, long productId);
    }
}
