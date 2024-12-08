using Lemoo_pos.Messages;
using Lemoo_pos.Services.Interfaces;
using MassTransit;

namespace Lemoo_pos.Consumers
{
    public class SaveSearchCustomerConsumer : IConsumer<SaveSearchCustomerMessage>
    {
        private readonly IElasticsearchService _elasticsearchService;

        public SaveSearchCustomerConsumer(IElasticsearchService elasticsearchService)
        {
            _elasticsearchService = elasticsearchService;
        }
        public async Task Consume(ConsumeContext<SaveSearchCustomerMessage> context)
        {
            System.Console.WriteLine("Start save search customer");
            await _elasticsearchService.SaveDocumentById(context.Message, context.Message.Id.ToString(), "customers");
            System.Console.WriteLine("End save search customer");
        }
    }
}