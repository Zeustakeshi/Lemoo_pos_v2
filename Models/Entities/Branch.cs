using System.ComponentModel.DataAnnotations;

namespace Lemoo_pos.Models.Entities
{
    public class Branch : BaseEntity
    {
        public required string Name { get; set; }
        
        public required Store Store { get; set; }

        public required long StoreId { get; set; }
        public bool? IsDefaultBranch { get; set; } = false;
        public string? Email { get; set; }
        public string? Phone { get; set; }

        public bool? IsActive { get; set; } = true;

        public string? Province { get; set; }

        public string? District { get; set; }

        public string? Ward { get; set; }


        public int? ProvinceCode { get; set; }

        public int? DistrictCode { get; set; }

        public int? WardCode { get; set; }

    }
}