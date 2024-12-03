using Lemoo_pos.Data;
using Lemoo_pos.Services;
using Lemoo_pos.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Lemoo_pos.ViewComponents
{
    public class RoleSelectViewComponent : ViewComponent
    {
        private readonly IAuthorityService _authorityService;

        public RoleSelectViewComponent(IAuthorityService authorityService)
        {
            _authorityService = authorityService;
        }
        public IViewComponentResult Invoke()
        {
            return View(_authorityService.GetAllAuthorities());
        }
    }
}