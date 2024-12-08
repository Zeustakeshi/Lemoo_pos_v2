using Lemoo_pos.Messages;
using Lemoo_pos.Services.Interfaces;
using MassTransit;

namespace Lemoo_pos.Consumers
{
    public class CreateAuthorityPermissionConsumer : IConsumer<Batch<CreateAuthorityPermissionMessage>>
    {
        private readonly IAuthorityService _authorityService;
        public CreateAuthorityPermissionConsumer(IAuthorityService authorityService)
        {
            _authorityService = authorityService;
        }

        public async Task Consume(ConsumeContext<Batch<CreateAuthorityPermissionMessage>> context)
        {
            var permissionGroupedByAuthority = context.Message
                .GroupBy(permssion => permssion.Message.AuthorityId)
                .Select(group => new
                {
                    AuthorityId = group.Key,
                    Permissions = group.Select(g => g.Message.permissionType).ToList()
                });
            foreach (var group in permissionGroupedByAuthority)
            {
                await _authorityService.SavePermissionBatch(group.AuthorityId, group.Permissions);
            }
        }
    }

}