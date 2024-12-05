using Lemoo_pos.Services;
using Lemoo_pos.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Lemoo_pos.Controllers
{
    [Route("pos/")]
    public class PosController : AuthenticationBaseController
    {

        private readonly IAuthService _authService;
        private readonly ISessionService _sessionService;
        public PosController(IAuthService authService, ISessionService sessionService)
        {
            _authService = authService;
            _sessionService = sessionService;
        }

        public IActionResult Index()
        {
            long storeId = _sessionService.GetStoreIdSession();
            long accountId = _sessionService.GetAccountIdSession();

            string token = _authService.GetnerateAuthorizationToken(accountId, storeId);

            Response.Cookies.Append("access_token", token, new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddDays(2),
                HttpOnly = false,
                Secure = false,
            });
            return View();
        }
    }
}
