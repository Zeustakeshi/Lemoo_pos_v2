using Lemoo_pos.Models.Entities;
using Lemoo_pos.Models.ViewModels;
using Lemoo_pos.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Lemoo_pos.Controllers
{
	[Route("/branches")]
	public class BranchController : AuthenticationBaseController
	{
		private readonly IBranchService _branchService;

		public BranchController (IBranchService branchService)
		{
			_branchService = branchService;
		}

        public IActionResult Index()
		{
			return View(_branchService.GetAllBranch());
		}

		[HttpPut("{branchId}/update")]
		public void UpdateBranch (long branchId, [FromBody] SaveBranchViewModel model)
		{
			try
			{
				_branchService.UpdateBranch(branchId, model);

			}catch (Exception ex)
			{
                Console.WriteLine(ex.Message);
                throw;
			}
		}

        [HttpPost("create")]
        public ActionResult CreateBranch([FromBody] SaveBranchViewModel model)
        {
            try
            {
				Response.StatusCode = 201;

				return Json(_branchService.CreateBranch(model));

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
				return Json("Tạo chi nhánh thất bại");
            }
        }
    }
}
