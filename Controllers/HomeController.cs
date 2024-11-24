using Lemoo_pos.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Lemoo_pos.Controllers
{
	public class HomeController : Controller
	{

		[HttpGet]
		public IActionResult Index()
		{

			ViewBag.UserName = HttpContext.Session.GetString("UserName");
			ViewBag.Email = HttpContext.Session.GetString("Email");
            ViewBag.Avatar = HttpContext.Session.GetString("Avatar");	

            return View();

		}

	}
}
