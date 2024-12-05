using Lemoo_pos.Models.Dto;
using Lemoo_pos.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nest;
using System.Net;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
        private readonly IBranchService _branchService;
        private readonly ICustomerService _customerService;

        public ApiPosController(
            IProductService productService,
            ISearchService searchService,
            IOrderService orderService,
            IAccountService accountService,
            IBranchService branchService,
            ICustomerService customerService
        )
        {
            _productService = productService;
            _searchService = searchService;
            _orderService = orderService;
            _accountService = accountService;
            _branchService = branchService;
            _customerService = customerService;
        }

        /*
            ================ START STORE INFO   ==================
       */

        [HttpGet("account-info")]
        public IActionResult GetAccountInfo()
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

        /*
              ================ END STORE INFO   ==================
      */

        /*
            ================ START PRODUCT   ==================
       */

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
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(ex.Message);
            }
        }

        [HttpGet("products/_count")]
        public IActionResult GetProductCount()
        {
            try
            {
                string storeIdString = User.Claims
                    .FirstOrDefault(c => c.Type == "storeId")?.Value ??
                    throw new Exception("Invalid jwt token.");
                long productCount = _productService.GetProductCount(Convert.ToInt64(storeIdString));
                return Json(productCount);
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(ex.Message);
            }
        }

        /*
                ================ END PRODUCT   ==================
        */

        /*
            ================ START BRANCH   ==================
       */

        [HttpGet("branches")]
        public IActionResult GetAllBranch()
        {
            try
            {
                string storeIdString = User.Claims
                    .FirstOrDefault(c => c.Type == "storeId")?.Value ??
                throw new Exception("Invalid jwt token.");

                return Json(_branchService.GetAllBranchByStoreId(Convert.ToInt64(storeIdString)));
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(ex.Message);
            }
        }

        /*
                ================ END BRANCH   ==================
        */
        /*
              ================ START ORDER   ==================
        */

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

                OrderResponseDto response = _orderService.CreateOrder(
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
        public IActionResult CreateOrderBatch([FromBody] List<CreateOrderDto> dto)
        {
            try
            {
                string accountIdString = User.Claims
                    .FirstOrDefault(c => c.Type == "accountId")?.Value ??
                    throw new Exception("Invalid jwt token.");

                string storeIdString = User.Claims
                    .FirstOrDefault(c => c.Type == "storeId")?.Value ??
                    throw new Exception("Invalid jwt token.");

                _orderService.CreateOrderBatch(
                   dto,
                   Convert.ToInt64(storeIdString),
                   Convert.ToInt64(accountIdString)
               );

                Response.StatusCode = 201;

                return Json("Sync order sucess");
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(ex.Message);
            }
        }

        /*
            ================ END ORDER   ==================
        */

        /*
          ================  START SEARCH  ==================
      */

        [HttpGet("products/search")]
        public IActionResult SearchProduct([FromQuery] string query, [FromQuery] long branchId)
        {
            try
            {
                string storeIdString = User.Claims
                    .FirstOrDefault(c => c.Type == "storeId")?.Value ??
                    throw new Exception("Invalid jwt token.");

                return Json(_searchService.SearchProduct(Convert.ToInt64(storeIdString), branchId, query));
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(ex.Message);
            }
        }

        [HttpGet("customers/search")]
        public IActionResult SearchCustomer([FromQuery] string query)
        {
            try
            {
                string storeIdString = User.Claims
                    .FirstOrDefault(c => c.Type == "storeId")?.Value ??
                    throw new Exception("Invalid jwt token.");

                var response = _searchService.SearchCustomer(Convert.ToInt64(storeIdString), query);
                return Json(response);
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(ex.Message);
            }
        }

        /*
        ================  END SEARCH ==================
      */

        /*
           ================  START CUSTOMER  ==================
       */

        [HttpPost("customers")]
        public IActionResult CreateCustomer([FromBody] CreateCustomerDto dto)
        {
            try
            {
                string accountIdString = User.Claims
                    .FirstOrDefault(c => c.Type == "accountId")?.Value ??
                    throw new Exception("Invalid jwt token.");
                string storeIdString = User.Claims
                    .FirstOrDefault(c => c.Type == "storeId")?.Value ??
                    throw new Exception("Invalid jwt token.");
                var response = _customerService.CreateCustomer(
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

        [HttpGet("customers/_count")]
        public IActionResult GetCustomerCount()
        {
            try
            {
                string storeIdString = User.Claims
                    .FirstOrDefault(c => c.Type == "storeId")?.Value ??
                    throw new Exception("Invalid jwt token.");
                return Json(_customerService.GetCustomerCount(Convert.ToInt64(storeIdString)));
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(ex.Message);
            }
        }

        /*
          ================  END CUSTOMER ==================
        */

    }
}
