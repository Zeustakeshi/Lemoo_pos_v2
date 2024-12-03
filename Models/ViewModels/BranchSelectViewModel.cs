using Lemoo_pos.Models.Entities;

namespace Lemoo_pos.Models.ViewModels
{
    public class BranchSelectViewModel
    {
        public bool AllowMultiSelect { get; set; } = true;
        public required List<Branch> Branches { get; set; }
    }
}
