namespace Lemoo_pos.Models.ViewModels
{
    public class CreateStaffViewModel
    {
        public required string Name { get; set; }
        public required string Phone { get; set; }
        public required string Email { get; set; }
        public required string Gender { get; set; }
        public required List<long> Roles { get; set; }
        public required long Branch { get; set; }

    }
}
