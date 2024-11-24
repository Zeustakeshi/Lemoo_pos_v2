using System.ComponentModel.DataAnnotations;

namespace Lemoo_pos.Models.Entities
{
    public class Staff : BaseEntity
    {
        public required Account Account { get; set;}

        public required Branch Branch { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string? Address { get; set; }
    }
}