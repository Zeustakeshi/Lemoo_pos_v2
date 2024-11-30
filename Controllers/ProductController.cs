using Lemoo_pos.Models.Entities;
using Lemoo_pos.Models.ViewModels;
using Lemoo_pos.Services;
using Lemoo_pos.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Lemoo_pos.Controllers
{

    [Route("products")]
    public class ProductController : AuthenticationBaseController
    {
        private readonly IProductService _productService;

        public ProductController (IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(_productService.GetAllProduct());
        }

        [HttpGet("create")]
        public IActionResult CreateProduct()
        {
            return View();
        }


        [HttpPost("create")]
        public  async Task<IActionResult>  CreateProduct( [FromForm] string product, [FromForm] IFormFile image)
        {
            var productData = System.Text.Json.JsonSerializer.Deserialize<CreateProductViewModel>(product);

            if (productData == null) { return BadRequest("Product is null"); }

            try {
                await _productService.CreateProduct(productData, image);
                return Ok("Product created successfully");
            }   
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        [HttpGet("{productId}/variants")]
        public IActionResult ProductVariants (long productId)
        {
            return View(_productService.GetAllVariants(productId));
        }

        [HttpGet("{productId}/variants/{variantId}")]
        public IActionResult ProductVariantDetail(long productId, long variantId)
        {
            return View(_productService.GetProductVariantByIdAndProductID(variantId, productId));
        }



    }
}
