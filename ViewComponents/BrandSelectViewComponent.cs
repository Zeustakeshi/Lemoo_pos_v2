using Lemoo_pos.Data;
using Lemoo_pos.Services;
using Lemoo_pos.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Lemoo_pos.ViewComponents
{
    public class BrandSelectViewComponent : ViewComponent
    {
        private readonly IBrandService _brandService;

        public BrandSelectViewComponent(IBrandService brandService)
        {
            _brandService = brandService;
        }
        public IViewComponentResult Invoke()
        {
            return View(_brandService.GetAllBrand());
        }
    }
}