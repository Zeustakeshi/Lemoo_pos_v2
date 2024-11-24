using Lemoo_pos.Models;
using Lemoo_pos.Models.Entities;
using Lemoo_pos.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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

		[HttpGet]
		[Route("login")]
		public IActionResult Login()
		{
			return View();
		}


        [HttpPost]
        [Route("login")]
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


        [HttpGet]
        [Route("logout")]
        public IActionResult Logout ()
		{
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
		}


        [HttpGet]
		[Route("register")]
		public IActionResult Register()
		{
			return View();
		}


		[HttpPost]
        [Route("register")]
		public  IActionResult Register (RegisterStoreViewModel model)
		{
			if (!ModelState.IsValid)
			{
                return View(model);
			}

            try
            {
                _authService.CreateAccount(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ViewData["Error_message"] = ex.Message;
                return View(model);
            }

			return RedirectToAction("ConfirmMail", new { email = model.Email, name = model.Name }) ;
        }


        [HttpGet]
		[Route("recoverpw")]
		public IActionResult RecoverPassword()
		{
			return View();
		}

		[HttpGet]
		[Route("confirm-mail")]
		public IActionResult ConfirmMail(string email, string name)
		{
			ViewBag.email = email;
			ViewBag.name = name;

			if (email == null || name == null) return RedirectToAction("Login");

			return View();
		}

        [HttpGet]
        [Route("verify-email")]
        public IActionResult VerifyEmail(string email, string code)
        {
			if (email == null || code == null) return RedirectToAction("Login");

			Account account =  _authService.VerifyEmail(email, code);


			if (account == null)
			{
                ViewBag.verifySuccess = account != null;
                return View();
			}

			HttpContext.Session.SetString("UserName", account.Name);
            HttpContext.Session.SetString("Email", account.Email);
            HttpContext.Session.SetString("Avatar", account.Avatar ?? "");

            return Redirect("/");
        }

    }
}
