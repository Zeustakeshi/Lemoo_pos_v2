namespace Lemoo_pos.Models.ViewModels
{
    public class CreateRoleViewModel
    {
        public required string Name { get; set; }

        public required string Description { get; set; }

        public required List<string> Permissions { get; set; }
    }
}
