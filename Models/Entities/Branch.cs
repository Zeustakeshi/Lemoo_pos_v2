using System.ComponentModel.DataAnnotations;

namespace Lemoo_pos.Models.Entities
{
    public class Branch : BaseEntity
    {
        public required string Name { get; set; }
        
        public required Store Store { get; set; }

        public string? Address { get; set; }

    }
}