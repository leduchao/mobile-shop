
using System.ComponentModel.DataAnnotations;

namespace MobileWeb.Areas.Identity.Models.Account;

public class ForgotPasswordViewModel
{
    [Required(ErrorMessage = "Hãy nhập email đã đăng ký!")]
    [EmailAddress(ErrorMessage = "Không đúng định dạng email!")]
    [StringLength(100, ErrorMessage = "{0} phải có ít nhất {2} ký tự!", MinimumLength = 3)]
    public string? Email { get; set; }
}
