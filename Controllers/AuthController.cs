using Lemoo_pos.Common.Enums;
using Lemoo_pos.Models.Entities;
using Lemoo_pos.Models.ViewModels;
using Lemoo_pos.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Net.Http;
using System.Security.Principal;

namespace Lemoo_pos.Controllers
{
    [Route("auth")]
	public class AuthController : Controller
	{

		private readonly IAuthService _authService;

		public AuthController (IAuthService authService)
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

                HttpContext.Session.SetString("UserName", account.Name);
                HttpContext.Session.SetString("Email", account.Email);
                HttpContext.Session.SetString("Avatar", account.Avatar ?? "");

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
        public IActionResult Logout ()
		{
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
		}


        [HttpGet("register")]
		public IActionResult Register()
		{
			return View();
		}


		[HttpPost("register")]
		public async Task<IActionResult> Register (RegisterStoreViewModel model)
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
            string code =  HttpContext.Session.GetString("code");
            string typeString = HttpContext.Session.GetString("type");

            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(typeString)) return RedirectToAction("Login");

            OtpType type = (OtpType)Enum.Parse(typeof(OtpType), typeString);

            try
            {
                if (type.Equals(OtpType.ACCOUNT_CREATION))
                {
                    Account account = _authService.VerifyAccountCreationOtp(code, plainOtp);
                    HttpContext.Session.SetString("UserName", account.Name);
                    HttpContext.Session.SetString("Email", account.Email);
                    HttpContext.Session.SetString("Avatar", account.Avatar ?? "");
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
        public async Task<IActionResult> ResendOtp ()
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
                return RedirectToAction("VerifyOtp", new { resendSuccessMaessage = "Gửi lại otp thành công"});
            }
            catch (Exception ex)
            {
                return RedirectToAction("VerifyOtp", new { resendErrorMessage = "Gửi lại otp thất bại. "});
            }
        }

    }
}
