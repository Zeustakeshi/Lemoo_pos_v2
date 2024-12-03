using Lemoo_pos.Models.Dto;
using Lemoo_pos.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nest;
using System.Net;

namespace Lemoo_pos.Controllers
{

    [Route("api/pos")]
    [Authorize]
    public class ApiPosController : Controller
    {
        private readonly IProductService _productService;
        private readonly ISearchService _searchService;
        private readonly IOrderService _orderService;
        private readonly IAccountService _accountService;
        public ApiPosController (
            IProductService productService, 
            ISearchService searchService, 
            IOrderService orderService, 
            IAccountService accountService
        ) { 
            _productService = productService;
            _searchService = searchService;
            _orderService = orderService;
            _accountService = accountService;
        }

        [HttpGet("account-info")]
        public IActionResult GetAccountInfo ()
        {
            try
            {
                string accountIdString = User.Claims
                    .FirstOrDefault(c => c.Type == "accountId")?.Value ??
                    throw new Exception("Invalid jwt token.");

                string storeIdString = User.Claims
                    .FirstOrDefault(c => c.Type == "storeId")?.Value ??
                    throw new Exception("Invalid jwt token.");

                return Json(_accountService.GetAccountById(Convert.ToInt64(accountIdString)));
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(ex.Message);
            }
        }


        [HttpPost("products")]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto dto)
        {
            try
            {
                string accountIdString = User.Claims
                    .FirstOrDefault(c => c.Type == "accountId")?.Value ?? 
                    throw new Exception("Invalid jwt token.");


                string storeIdString = User.Claims
                    .FirstOrDefault(c => c.Type == "storeId")?.Value ?? 
                    throw new Exception("Invalid jwt token."); 


                ProductResponseDto response = await _productService.CreateProduct(
                    dto, 
                    Convert.ToInt64(accountIdString), 
                    Convert.ToInt64(storeIdString)
                );

                Response.StatusCode = 201;

                return Json(response);
            }catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(ex.Message);
            }
        }

        [HttpGet("products/search")]
        public  IActionResult SearchProduct([FromQuery] string query)
        {
            try
            {
                string storeIdString = User.Claims
                    .FirstOrDefault(c => c.Type == "storeId")?.Value ??
                    throw new Exception("Invalid jwt token.");

                return Json(_searchService.SearchProduct( Convert.ToInt64(storeIdString),  query));
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(ex.Message);
            }
        }

        [HttpPost("orders")]
        public IActionResult CreateOrder([FromBody] CreateOrderDto dto)
        {
            try
            {
                string accountIdString = User.Claims
                    .FirstOrDefault(c => c.Type == "accountId")?.Value ??
                    throw new Exception("Invalid jwt token.");


                string storeIdString = User.Claims
                    .FirstOrDefault(c => c.Type == "storeId")?.Value ??
                    throw new Exception("Invalid jwt token.");

                OrderResponseDto response =  _orderService.CreateOrder(
                    dto,
                    Convert.ToInt64(storeIdString),
                    Convert.ToInt64(accountIdString)
                );

                Response.StatusCode = 201;

                return Json(response);
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(ex.Message);
            }
        }


        [HttpPost("orders/batch")]
        public IActionResult CreateOrderBatch ([FromBody] List<CreateOrderDto> dto)
        {
            Console.WriteLine(dto);
            return Json("oke");
        }
    }
}
