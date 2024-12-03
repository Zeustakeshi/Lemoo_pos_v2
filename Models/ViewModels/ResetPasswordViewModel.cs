using System.ComponentModel.DataAnnotations;

namespace Lemoo_pos.Models.ViewModels
{
    public class ResetPasswordViewModel
    {

        [Required(ErrorMessage = "Mật khẩu không được bỏ trống.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        public required string Password { get; set; }

        [Required(ErrorMessage = "Xác nhận mật khẩu không được bỏ trống.")]
        public required string ConfirmPassword { get; set; } = null!;

        public  string Token { get; set; }
    }
}
