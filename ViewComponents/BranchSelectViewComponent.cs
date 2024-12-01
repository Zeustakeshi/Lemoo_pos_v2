using Lemoo_pos.Data;
using Lemoo_pos.Services;
using Lemoo_pos.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Lemoo_pos.ViewComponents
{
    public class BranchSelectViewComponent : ViewComponent
    {
        private readonly IBranchService _branchService;

        public BranchSelectViewComponent(IBranchService branchService)
        {
            _branchService = branchService;
        }
        public IViewComponentResult Invoke()
        {
            return View(_branchService.GetAllBranch());
        }
    }
}