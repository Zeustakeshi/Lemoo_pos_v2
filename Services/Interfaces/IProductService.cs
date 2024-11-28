using Lemoo_pos.Models.ViewModels;

namespace Lemoo_pos.Services.Interfaces
{
    public interface IProductService
    {
        CreateProductViewModel CreateProduct(CreateProductViewModel model);
    }
}
