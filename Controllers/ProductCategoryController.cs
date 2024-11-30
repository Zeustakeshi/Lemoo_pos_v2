using Lemoo_pos.Models.Entities;
using Lemoo_pos.Models.ViewModels;
using Lemoo_pos.Services;
using Lemoo_pos.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Lemoo_pos.Controllers
{
    [Route("categories")]
    public class ProductCategoryController : AuthenticationBaseController
    {

        private readonly IProductCategoryService _productCategoryService;

        public ProductCategoryController (IProductCategoryService productCategoryService)
        {
            _productCategoryService = productCategoryService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(_productCategoryService.GetAllCategories());
        }

        [HttpGet("create")]
        public IActionResult CreateCategory()
        {
            return View();
        }


        [HttpPost("create")]
        public IActionResult CreateCategory ([FromForm] string category)
        {
            var categoryData = System.Text.Json.JsonSerializer.Deserialize<CreateCategoryViewModel>(category);

            if (categoryData == null) { return BadRequest("Product is null"); }

            try
            {
                _productCategoryService.CreateCategory(categoryData);
                return Ok("Category created successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
