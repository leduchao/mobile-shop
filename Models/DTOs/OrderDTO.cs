using MobileWeb.Models.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MobileWeb.Models.DTOs;

public class OrderDTO
{
	//public int Id { get; set; }
	public int Quantity { get; set; }

	public string Color { get; set; } = string.Empty;

	public virtual CartItem CartItem { get; set; } = new();

	public double TotalPrice { get; set; }

    [Required(ErrorMessage = "Hãy cho chúng tôi biết tên của bạn!")]
	[StringLength(100, ErrorMessage = "Tên phải dài từ 3 đến 100 ký tự!", MinimumLength = 3)]
    public string Firstname { get; set; } = string.Empty;

    [Required(ErrorMessage = "Hãy cho chúng tôi biết tên của bạn!")]
    [StringLength(100, ErrorMessage = "Tên phải dài từ 3 đến 100 ký tự!", MinimumLength = 3)]
    public string LastName { get; set; } = string.Empty;

	public string FullName => $"{Firstname} {LastName}";

	[Required(ErrorMessage = "Làm sao chúng tôi có thể vận chuyển hàng khi không biết địa chỉ của bạn!")]
	public string Address { get; set; } = string.Empty;

	[Required(ErrorMessage = "Hãy cho chúng tôi biết số điện thoại của bạn của bạn!")]
	public string PhoneNumber { get; set; } = string.Empty;

	public PaymentMethods PaymentMethod { get; set; }

}
