using Lemoo_pos.Messages;
using Lemoo_pos.Services.Interfaces;
using MassTransit;

namespace Lemoo_pos.Consumers
{
    public class UpdateProductBrandConsumer : IConsumer<UpdateProductBrandMessage>
    {
        private readonly IBrandService _brandService;
        public UpdateProductBrandConsumer(IBrandService brandService)
        {
            _brandService = brandService;
        }

        public async Task Consume(ConsumeContext<UpdateProductBrandMessage> context)
        {
            await _brandService.UpdateProductBrand(context.Message.ProductId, context.Message.BrandId);
        }
    }

}