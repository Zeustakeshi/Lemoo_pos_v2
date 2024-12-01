using Lemoo_pos.Models.Entities;
using Lemoo_pos.Models.ViewModels;

namespace Lemoo_pos.Services.Interfaces
{
    public interface IBrandService
    {
        List<Brand> GetAllBrand();
        Brand CreateBrand (CreateBrandViewModel model);
    }
}
