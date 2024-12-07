using Lemoo_pos.Areas.Api.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lemoo_pos.Controllers
{
    [Authorize]
    [Route("api/test")]
    public class TestController : Controller
    {

        [HttpPost("test-1")]
        public async Task<IActionResult> Index([FromBody] List<CreateOrderDto> dto)
        {
            foreach (var item in dto)
            {
                Console.WriteLine(item.ToString());
            }
            await Task.Delay(5000);
            return Json(dto);
        }

        [HttpGet]
        public IActionResult Details()
        {
            return Json("oke");
        }
    }
}
