using Lemoo_pos.Models.Entities;
using Lemoo_pos.Models.ViewModels;

namespace Lemoo_pos.Services.Interfaces
{
    public interface IInventoryService
    {
        void UpdateInventory(long id, UpdateVariantInventoryViewModel model);
        List<InventoryHistoryViewModel> GetInventoryHistories(long productVariantId);

        Task<Inventory> CreateInventory(long productVariantId, long branchId, long quantity, long available, long staffId);
    }
}
