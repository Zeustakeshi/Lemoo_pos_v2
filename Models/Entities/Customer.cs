using Lemoo_pos.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace Lemoo_pos.Models.Entities
{
    public class Customer : BaseEntity
    {
        public required Store Store { get; set; }
        public required long StoreId { get; set; }
        public required Staff Staff { get; set; }
        public required long StaffId { get; set; }
        public required string Name { get; set; }
        public required string Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public Gender? Gender { get; set; }
        public string? Province { get; set; }
        public string? District { get; set; }
        public string? Ward { get; set; }
        public int? ProvinceCode { get; set; }
        public int? DistrictCode { get; set; }
        public int? WardCode { get; set; }
        public List<Order> Orders { get; set; } = [];
    }
}