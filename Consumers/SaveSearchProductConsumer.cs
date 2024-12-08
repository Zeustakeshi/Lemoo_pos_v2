using Lemoo_pos.Messages;
using Lemoo_pos.Services.Interfaces;
using MassTransit;

namespace Lemoo_pos.Consumers
{
    public class SaveSearchProductConsumer : IConsumer<SaveSearchProductMessage>
    {
        private readonly IElasticsearchService _elasticsearchService;

        public SaveSearchProductConsumer(IElasticsearchService elasticsearchService)
        {
            _elasticsearchService = elasticsearchService;
        }

        public async Task Consume(ConsumeContext<SaveSearchProductMessage> context)
        {
            System.Console.WriteLine("start save search product");
            await _elasticsearchService.SaveDocumentById(context.Message, context.Message.Id.ToString(), "products");
            System.Console.WriteLine("end save search product");

        }
    }
}