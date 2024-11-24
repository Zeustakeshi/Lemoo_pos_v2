using System.ComponentModel.DataAnnotations;

namespace Lemoo_pos.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Địa chỉ email không được bỏ trống.")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không đúng định dạng.")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được bỏ trống.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        public required string Password { get; set; }
    }
}
