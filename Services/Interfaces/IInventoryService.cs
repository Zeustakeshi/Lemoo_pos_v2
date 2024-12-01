using Lemoo_pos.Models.Entities;
using Lemoo_pos.Models.ViewModels;

namespace Lemoo_pos.Services.Interfaces
{
    public interface IInventoryService
    {

        Inventory CreateInventory(ProductVariant productVariant, Branch branch, long quantity, long available);
        void UpdateInventory(long id, UpdateVariantInventoryViewModel model);
        List<InventoryHistoryViewModel> GetInventoryHistories(long productVariantId);
    }
}
