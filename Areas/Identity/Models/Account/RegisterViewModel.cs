
using System.ComponentModel.DataAnnotations;

namespace MobileWeb.Areas.Identity.Models.Account;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Phải nhập {0}")]
    [EmailAddress(ErrorMessage = "Sai định dạng Email")]
    [Display(Name = "Email")]
    public string? Email { get; set; }


    [Required(ErrorMessage = "Phải nhập {0}")]
    [StringLength(100, ErrorMessage = "{0} phải dài từ {2} ký tự trở lên.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu")]
    public string? Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Nhập lại mật khẩu")]
    [Compare("Password", ErrorMessage = "Mật khẩu lặp lại không chính xác.")]
    public string? ConfirmPassword { get; set; }

    [Display(Name = "Tên tài khoản")]
    [Required(ErrorMessage = "Phải nhập {0}")]
    [StringLength(100, ErrorMessage = "{0} phải dài từ {2} đến {1} ký tự.", MinimumLength = 3)]
    public string? UserName { get; set; }

}
