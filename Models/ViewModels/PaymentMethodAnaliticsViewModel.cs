using System.ComponentModel.DataAnnotations;

namespace Lemoo_pos.Models.ViewModels
{
    public class PaymentMethodAnaliticsViewModel
    {
        public required string PaymentMethod { get; set; }
        public required long TotalOrder { get; set; }
    }
}
