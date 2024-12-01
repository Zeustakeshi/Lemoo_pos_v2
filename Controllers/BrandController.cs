using Lemoo_pos.Models.ViewModels;
using Lemoo_pos.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Lemoo_pos.Controllers
{
    [Route("brands")]
    public class BrandController : AuthenticationBaseController
    {

        private readonly IBrandService _brandService;

        public BrandController(IBrandService brandService) 
        {
            _brandService = brandService;
        }


        [HttpPost]
        public JsonResult CreateBrand ([FromBody] CreateBrandViewModel model)
        {
            try
            {
                return Json(_brandService.CreateBrand(model));
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(ex.Message);
            }
        }
    }
}
