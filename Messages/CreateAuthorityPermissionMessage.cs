using Lemoo_pos.Common.Enums;

namespace Lemoo_pos.Messages
{
    public class CreateAuthorityPermissionMessage
    {
        public required long AuthorityId { get; set; }
        public required PermissionType permissionType;
    }
}