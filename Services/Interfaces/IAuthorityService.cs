using Lemoo_pos.Common.Enums;
using Lemoo_pos.Models;
using Lemoo_pos.Models.Entities;
using Lemoo_pos.Models.ViewModels;
using System.Xml.Serialization;

namespace Lemoo_pos.Services.Interfaces
{
    public interface IAuthorityService
    {
        Task<Authority> InitNewStoreAuthority(Store store);

        Task CreateRole(CreateRoleViewModel model);

        List<Authority> GetAllAuthorities();

        Task<Authority> CreateAuthorityAsync(CreateRoleViewModel model, long storeId, bool? hasAllPermission = false);

        Task SavePermissionBatch(long authorityId, List<PermissionType> permissions);
    }
}
