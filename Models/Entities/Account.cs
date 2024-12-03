using System.ComponentModel.DataAnnotations;

namespace Lemoo_pos.Models.Entities
{
    public class Account : BaseEntity
    {
        public required string Name { get; set; }

        public required string Email { get; set; }

        public required string Phone { get; set;}

        public required string Password { get; set; }

        public string? Avatar { get; set; }

        public required List<AccountAuthority> Authorities;

        public required Store Store { get; set; }

        public required bool IsActive { get; set; }
    }
}