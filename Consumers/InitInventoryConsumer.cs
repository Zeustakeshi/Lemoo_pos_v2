using Lemoo_pos.Messages;
using Lemoo_pos.Services.Interfaces;
using MassTransit;

namespace Lemoo_pos.Consumers
{
    public class InitInventoryConsumer : IConsumer<InitInventoryMessage>
    {

        private readonly IInventoryService _inventoryService;

        public InitInventoryConsumer(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        public async Task Consume(ConsumeContext<InitInventoryMessage> context)
        {
            var message = context.Message;
            System.Console.WriteLine("Inventory processing .............................");
            await _inventoryService.CreateInventory(
                 productVariantId: message.ProductVariantId,
                 branchId: message.BranchId,
                 available: message.Available,
                 quantity: message.Quantity,
                 staffId: message.StaffId
            );
            System.Console.WriteLine("End Inventory processing .............................");
        }
    }
}