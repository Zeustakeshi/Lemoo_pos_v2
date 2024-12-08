using Lemoo_pos.Messages;
using Lemoo_pos.Services.Interfaces;
using MassTransit;

namespace Lemoo_pos.Consumers
{
    public class CreateAuthorityConsumer : IConsumer<CreateAuthorityMessage>
    {
        private readonly IAuthorityService _authorityService;
        public CreateAuthorityConsumer(IAuthorityService authorityService)
        {
            _authorityService = authorityService;
        }

        public async Task Consume(ConsumeContext<CreateAuthorityMessage> context)
        {
            await _authorityService.CreateAuthorityAsync(context.Message, context.Message.StoreId);
        }
    }

}