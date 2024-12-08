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
        public Task Consume(ConsumeContext<Batch<CreateOrderBatchMessage>> context)
        {
            List<CreateOrderDto> data = [];
            foreach (var message in context.Message)
            {
                data.Add(message.Message);
            }
            // _orderServiceApi.CreateOrderBatch(data);
            throw new NotImplementedException();
        }
    }
}