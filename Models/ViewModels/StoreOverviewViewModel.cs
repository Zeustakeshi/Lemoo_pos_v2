using System.ComponentModel.DataAnnotations;

namespace Lemoo_pos.Models.ViewModels
{
    public class StoreOverviewViewModel
    {
        public required long TotalSales { get; set; }
        public required long TotalProducts { get; set; }
        public required long Revenue { get; set; }

    }
}
