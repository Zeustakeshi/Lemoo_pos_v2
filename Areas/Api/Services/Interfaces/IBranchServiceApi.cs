using Lemoo_pos.Areas.Api.Dto;
using Lemoo_pos.Models.Entities;
using Lemoo_pos.Models.ViewModels;

namespace Lemoo_pos.Areas.Api.Services.Interfaces
{
    public interface IBranchServiceApi
    {
        List<BranchResponseDto> GetAllBranchByStoreId(long storeId);
    }
}
