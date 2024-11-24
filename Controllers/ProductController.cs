using Microsoft.AspNetCore.Mvc;

namespace Lemoo_pos.Controllers
{

    [Route("products")]

    public class ProductController : Controller
    {

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("create")]
        public IActionResult CreateProduct()
        {
            return View();
        }
    }
}
