using Lemoo_pos.Areas.Api.Services.Interfaces;
using Lemoo_pos.Messages;
using MassTransit;

namespace Lemoo_pos.Consumers
{
    public class UpdateOrderCustomerConsumer : IConsumer<UpdateOrderCustomerMessage>
    {
        private readonly IOrderServiceApi _orderServiceApi;
        public UpdateOrderCustomerConsumer(IOrderServiceApi orderServiceApi)
        {
            _orderServiceApi = orderServiceApi; ;
        }
        public async Task Consume(ConsumeContext<UpdateOrderCustomerMessage> context)
        {
            System.Console.WriteLine("Start update order customer");
            await _orderServiceApi.UpdateOrderCustomer(context.Message.OrderId, context.Message.CustomerId);
            System.Console.WriteLine("End update order customer");
        }
    }
}