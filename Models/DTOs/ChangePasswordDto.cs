using System.ComponentModel.DataAnnotations;

namespace MobileWeb.Models.DTOs;

public class ChangePasswordDto
{
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phải nhập {0}")]
    [StringLength(100, ErrorMessage = "Mật khẩu phải có ít nhất {2} ký tự!", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Nhập mật khẩu hiện tại")]
    public string CurrentPassword { get; set; } = string.Empty;


    [Required(ErrorMessage = "Phải nhập {0}")]
	[StringLength(100, ErrorMessage = "Mật khẩu phải có ít nhất {2} ký tự!", MinimumLength = 6)]
	[DataType(DataType.Password)]
	[Display(Name = "Nhập mật khẩu mới")]
	public string NewPassword { get; set; } = string.Empty;

    [DataType(DataType.Password)]
	[Display(Name = "Nhập lại mật khẩu mới")]
	[Compare("NewPassword", ErrorMessage = "Bạn đã nhập lại mật khẩu không trùng khớp!")]
	public string ConfirmNewPassword { get; set; } = string.Empty;

}