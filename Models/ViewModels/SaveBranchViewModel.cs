namespace Lemoo_pos.Models.ViewModels
{
    public class SaveBranchViewModel
    {
        public required string Name { get; set; }

        public required string Email { get; set; }

        public required string Phone { get; set; }
        
        public string? Province { get; set;}

        public int? ProvinceCode { get; set;}

        public string? District { get; set; }

        public int? DistrictCode { get; set; }

        public string? Ward { get; set; }

        public int? WardCode { get; set; }

        public bool? IsActive { get; set; }
    }
}
