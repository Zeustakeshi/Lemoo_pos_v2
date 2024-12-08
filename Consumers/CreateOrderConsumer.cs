using Lemoo_pos.Areas.Api.Dto;
using Lemoo_pos.Areas.Api.Services.Interfaces;
using Lemoo_pos.Messages;
using MassTransit;

namespace Lemoo_pos.Consumers
{
    public class CreateOrderBatchConsumer : IConsumer<Batch<CreateOrderBatchMessage>>
    {
        private readonly IOrderServiceApi _orderServiceApi;
        public CreateOrderBatchConsumer(IOrderServiceApi orderServiceApi)
        {
            _orderServiceApi = orderServiceApi;
        }
        public async Task Consume(ConsumeContext<Batch<CreateOrderBatchMessage>> context)
        {
            var orderGroupedByStoreId = context.Message
              .GroupBy(order => new { storeId = order.Message.StoreId, accountId = order.Message.AccountId })
              .Select(group => new
              {
                  Keys = new
                  {
                      StoreId = group.Key.storeId,
                      AccountId = group.Key.accountId
                  },
                  Orders = group.Select((order) => new CreateOrderDto()
                  {
                      BranchId = order.Message.BranchId,
                      Change = order.Message.Change,
                      Items = order.Message.Items,
                      PaymentMethod = order.Message.PaymentMethod,
                      Total = order.Message.Total,
                      CustomerId = order.Message.CustomerId,
                      Description = order.Message.Description
                  }).ToList()
              });

            foreach (var group in orderGroupedByStoreId)
            {
                await _orderServiceApi.CreateOrderBatchAsync(group.Orders, group.Keys.StoreId, group.Keys.AccountId);
            }
        }
    }
}