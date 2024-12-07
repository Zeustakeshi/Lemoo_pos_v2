using Lemoo_pos.Common.Enums;
using Lemoo_pos.Models.Entities;
using Lemoo_pos.Models.ViewModels;
using Lemoo_pos.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;


namespace Lemoo_pos.Controllers
{
    [Route("auth")]
    public class AuthController : Controller
    {

        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }


        [HttpPost("login")]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                Account account = _authService.Login(model);
                return Redirect("/");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ViewData["Error_message"] = ex.Message;
                return View(model);
            }

        }


        [HttpGet("logout")]
        public IActionResult Logout()
        {
            _authService.Logout();
            return RedirectToAction("Login");
        }


        [HttpGet("register")]
        public IActionResult Register()
        {
            return View();
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterStoreViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                string otpCode = await _authService.CreateAccount(model);
                return RedirectToAction("VerifyOtp");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ViewData["Error_message"] = ex.Message;
                return View(model);
            }
        }


        [HttpGet("recoverpw")]
        public IActionResult RecoverPassword()
        {
            return View();
        }

        [HttpPost("recoverpw")]
        public async Task<IActionResult> RecoverPassword([FromForm] RecoverPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _authService.RecoverPassword(model);
                ViewData["Success_message"] =
                    "Chúng tôi đã gửi mail hướng dẫn lấy lại mật khẩu đến " + model.Email +
                    " vui lòng kiểm tra mail của bạn và làm theo hướng dẫn để lấy lại mật khẩu";

                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ViewData["Error_message"] = ex.Message;
                return View(model);
            }

        }


        [HttpGet("reset-password")]
        public IActionResult ResetPassword([FromQuery] string token)
        {
            if (token == null)
            {
                return RedirectToAction("Login");
            }
            ViewData["token"] = token;
            return View();
        }

        [HttpPost("reset-password")]
        public IActionResult ResetPassword([FromForm] ResetPasswordViewModel model)
        {
            ViewData["token"] = model.Token;
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!model.Password.Equals(model.ConfirmPassword))
            {
                ViewData["Error_message"] = "Mật khẩu và mật khẩu nhập lại không trùng khớp";
                return View(model);
            }

            try
            {
                _authService.ResetPassword(model, model.Token);
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ViewData["Error_message"] = ex.Message;
                return View(model);
            }

        }

        [HttpGet("verify-otp")]
        public IActionResult VerifyOtp(string? resendErrorMessage, string? resendSuccessMaessage)
        {
            ViewBag.resendErrorMessage = resendErrorMessage;
            ViewBag.resendSuccessMaessage = resendSuccessMaessage;
            ViewBag.name = HttpContext.Session.GetString("name");
            ViewBag.email = HttpContext.Session.GetString("email");

            return View();
        }

        [HttpPost("verify-otp")]
        public IActionResult VerifyOtpHandler(string plainOtp)
        {
            string code = HttpContext.Session.GetString("code");
            string typeString = HttpContext.Session.GetString("type");

            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(typeString)) return RedirectToAction("Login");

            OtpType type = (OtpType)Enum.Parse(typeof(OtpType), typeString);

            try
            {
                if (type.Equals(OtpType.ACCOUNT_CREATION))
                {
                    Account account = _authService.VerifyAccountCreationOtp(code, plainOtp);

                }

                HttpContext.Session.Remove("code");
                HttpContext.Session.Remove("type");
                HttpContext.Session.Remove("email");
                HttpContext.Session.Remove("name");

                return Redirect("/");
            }
            catch (Exception ex)
            {
                ViewBag.verifyErrorMessage = ex.Message;
                return View("VerifyOtp");
            }
        }

        [HttpGet("resend-otp")]
        public async Task<IActionResult> ResendOtp()
        {
            string code = HttpContext.Session.GetString("code");
            string typeString = HttpContext.Session.GetString("type");

            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(typeString)) return RedirectToAction("Login");

            OtpType type = (OtpType)Enum.Parse(typeof(OtpType), typeString);

            try
            {
                if (type.Equals(OtpType.ACCOUNT_CREATION))
                {
                    string otpCode = await _authService.ResendAccountCreationOtp(code);
                }
                return RedirectToAction("VerifyOtp", new { resendSuccessMaessage = "Gửi lại otp thành công" });
            }
            catch (Exception ex)
            {
                return RedirectToAction("VerifyOtp", new { resendErrorMessage = "Gửi lại otp thất bại. " });
            }
        }

    }
}
