using Lemoo_pos.Areas.Api.Dto;
using Lemoo_pos.Models.Entities;
using Lemoo_pos.Models.ViewModels;

namespace Lemoo_pos.Services.Interfaces
{
    public interface IBranchService
    {
        List<Branch> GetAllBranch();

        Branch CreateDefaultBranch(Store store, string email, string phone);

        Task<long> CreateBranchAsync(long storeId, string name, string email, string phone, bool isDefaultBranch);

        void UpdateBranch(long branchId, SaveBranchViewModel model);

        Branch CreateBranch(SaveBranchViewModel model);

        List<BranchResponseDto> GetAllBranchByStoreId(long storeId);
    }
}
