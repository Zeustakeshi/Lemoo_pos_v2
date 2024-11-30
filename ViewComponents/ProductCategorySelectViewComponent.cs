using Lemoo_pos.Data;
using Lemoo_pos.Services;
using Lemoo_pos.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Lemoo_pos.ViewComponents
{
    public class ProductCategorySelectViewComponent : ViewComponent
    {
        private readonly IProductCategoryService _productCategoryService;

        public ProductCategorySelectViewComponent (IProductCategoryService productCategoryService)
        {
            _productCategoryService = productCategoryService;
        }
        public IViewComponentResult Invoke()
        {
            return View(_productCategoryService.GetAllCategories());
        }
    }
}