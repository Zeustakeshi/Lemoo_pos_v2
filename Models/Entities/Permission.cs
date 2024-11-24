using Lemoo_pos.Common.Enums;

namespace Lemoo_pos.Models.Entities
{
    public class Permission : BaseEntity
    {
        public required PermissionType Type { get; set; }
    }
}
