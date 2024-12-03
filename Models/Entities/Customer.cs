using System.ComponentModel.DataAnnotations;

namespace Lemoo_pos.Models.Entities
{
    public class Customer : BaseEntity
    {
       public required string Name { get; set; }

        public  string? Address { get; set; }

        public string? Email { get; set; }  

        public string? Phone { get; set; }

        public required Staff Staff { get; set; }

        public required long StaffId { get; set; }

        public List<Order> Orders { get; set; } = [];
    }
}