using Lemoo_pos.Models.Entities;

namespace Lemoo_pos.Areas.Api.Dto
{
    public class AccountInfoResponseDto
    {
        public required long Id { get; set; }
        public required string Name { get; set; }

        public required string Email { get; set; }

        public required string Phone { get; set; }

        public string? Avatar { get; set; }

    }
}
