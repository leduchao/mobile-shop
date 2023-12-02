
using System.ComponentModel.DataAnnotations;

namespace MobileWeb.Areas.Identity.Models.Account;

public class ResetPasswordViewModel
{
    [Required(ErrorMessage = "Phải nhập {0}")]
    [StringLength(100, ErrorMessage = "Mật khẩu phải có ít nhất {2} ký tự!", MinimumLength = 3)]
    [DataType(DataType.Password)]
    [Display(Name = "Nhập mật khẩu mới")]
    public string? Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Nhập lại mật khẩu")]
    [Compare("Password", ErrorMessage = "Mật khẩu không trùng khớp!")]
    public string? ConfirmPassword { get; set; }
}
