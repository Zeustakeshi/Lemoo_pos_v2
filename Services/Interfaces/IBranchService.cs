using Lemoo_pos.Models.Entities;
using Lemoo_pos.Models.ViewModels;

namespace Lemoo_pos.Services.Interfaces
{
    public interface IBranchService
    {
        List<Branch> GetAllBranch();

        Branch CreateDefaultBranch(Store store, string email, string phone);

        void UpdateBranch (long branchId, SaveBranchViewModel model);

        Branch CreateBranch(SaveBranchViewModel model);
    }
}
