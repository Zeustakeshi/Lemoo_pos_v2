using Lemoo_pos.Models.Entities;
using Lemoo_pos.Models.ViewModels;
using Lemoo_pos.Services;
using Lemoo_pos.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Lemoo_pos.Controllers
{

    [Route("products")]

    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController (IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("create")]
        public IActionResult CreateProduct()
        {
            return View();
        }


        [HttpPost("create")]
        public async Task<IActionResult> CreateProduct( [FromForm] string product)
        {
            var productData = System.Text.Json.JsonSerializer.Deserialize<CreateProductViewModel>(product);

            if (productData == null) { return BadRequest("Product is null"); }


            return Ok(new { message = "Product created successfully", product = _productService.CreateProduct(productData) });
        }
    }
}
