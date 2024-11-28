using System.ComponentModel.DataAnnotations;

namespace Lemoo_pos.Models.ViewModels
{
    public class RegisterStoreViewModel
    {

        [Required(ErrorMessage = "Tên cửa hàng không được bỏ trống.")]
        [StringLength(200, ErrorMessage = "Tên cửa hàng quá dài, tối đa 200 ký tự.")]
        public required string StoreName { get; set; }


        [Required(ErrorMessage = "Tên chủ cửa hàng không được bỏ trống.")]
        [StringLength(100, ErrorMessage = "Tên chủ cửa hàng quá dài, tối đa 100 ký tự.")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Địa chỉ email không được bỏ trống.")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không đúng định dạng.")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được bỏ trống.")]
        [Phone(ErrorMessage = "Số điện thoại không đúng định dạng.")]
        [StringLength(15, ErrorMessage = "Số điện thoại quá dài, tối đa 15 ký tự.")]
        public required string Phone { get; set; }


        [Required(ErrorMessage = "Mật khẩu không được bỏ trống.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        public required string Password { get; set; }

    }
}
