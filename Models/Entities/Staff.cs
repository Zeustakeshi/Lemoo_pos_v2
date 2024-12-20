using Lemoo_pos.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace Lemoo_pos.Models.Entities
{
    public class Staff : BaseEntity
    {
        public required long AccountId { get; set; }
        public required Account Account { get; set; }

        public required Branch Branch { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string? Address { get; set; }
        public Gender? Gender { get; set; }

        public required StaffStatus Status { get; set; }
    }
}