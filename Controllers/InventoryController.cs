using Lemoo_pos.Models.ViewModels;
using Lemoo_pos.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Lemoo_pos.Controllers
{
    [Route("inventory")]
    public class InventoryController : AuthenticationBaseController
    {

        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService) {
            _inventoryService = inventoryService;
        }

        [HttpGet("{productVariantId}/history")]
        public ActionResult InventoryHistories(long productVariantId) {
            return View(_inventoryService.GetInventoryHistories(productVariantId));
        }

        [HttpPut("{inventoryId}")]
        public void UpdateInventoryVariant (long inventoryId, [FromBody] UpdateVariantInventoryViewModel model)
        {
            _inventoryService.UpdateInventory(inventoryId, model);
        }


    }
}
