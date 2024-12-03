using System.ComponentModel.DataAnnotations;

namespace Lemoo_pos.Models.ViewModels
{
    public class RecoverPasswordViewModel
    {
        [Required(ErrorMessage = "Địa chỉ email không được bỏ trống.")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không đúng định dạng.")]
        public required string Email { get; set; }
    }
}
