using Lemoo_pos.Models.Entities;
using Lemoo_pos.Models.ViewModels;

namespace Lemoo_pos.Services.Interfaces
{
    public interface IProductCategoryService
    {
        List<ProductCategory> GetAllCategories();

        void CreateCategory(CreateCategoryViewModel model); 
    }
}
