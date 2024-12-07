using Lemoo_pos.Models.Entities;
using Lemoo_pos.Models.ViewModels;

namespace Lemoo_pos.Services.Interfaces
{
    public interface IStaffService
    {
        List<Staff> GetAllStaff(long storeId);

        List<string> GetAllStaffStatus();

        void CreateStaff(CreateStaffViewModel model);
    }
}
