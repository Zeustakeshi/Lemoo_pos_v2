using Lemoo_pos.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace Lemoo_pos.Models.Entities
{
    public class Authority : BaseEntity
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required Store Store { get; set; }

        public required long StoreId { get; set; }

        public bool? HasAllPermission { get; set; } = false;

        public required List<AuthorityPermission> Permissions { get; set; }
    }
}