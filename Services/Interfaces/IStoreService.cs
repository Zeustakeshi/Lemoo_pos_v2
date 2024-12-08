using Lemoo_pos.Models.Entities;
using Lemoo_pos.Models.ViewModels;

namespace Lemoo_pos.Services.Interfaces
{
    public interface IStoreService
    {
        Task<Store> CreateNewStore(string name);

        StoreOverviewViewModel GetStoreOverview();
    }
}
