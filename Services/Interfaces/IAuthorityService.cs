using Lemoo_pos.Models;
using Lemoo_pos.Models.Entities;
using Lemoo_pos.Models.ViewModels;
using System.Xml.Serialization;

namespace Lemoo_pos.Services.Interfaces
{
    public interface IAuthorityService
    {
        void InitNewStoreAuthority(Store store);

        void CreateRole(CreateRoleViewModel model);

        List<Authority> GetAllAuthorities();
        
    }
}
