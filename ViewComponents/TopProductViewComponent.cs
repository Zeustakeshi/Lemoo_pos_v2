using Lemoo_pos.Data;
using Lemoo_pos.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Lemoo_pos.ViewComponents
{
    public class TopProductViewComponent : ViewComponent
    {

        private readonly IProductService _productService;

        public TopProductViewComponent(IProductService productService)
        {
            _productService = productService;
        }
        public IViewComponentResult Invoke()
        {
            var topProducts = _productService.GetTopProducts(4);
            return View(topProducts);
        }
    }
}