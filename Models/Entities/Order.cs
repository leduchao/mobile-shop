using Microsoft.AspNetCore.Identity;
using MobileWeb.Models.DTOs;
using System.ComponentModel.DataAnnotations;

namespace MobileWeb.Models.Entities;

public class Order
{
	[Key]
	public int Id { get; set; }

	public List<OrderItem> OrderItems { get; set; } = new();

	public Status Status { get; set; }

	public string? UserId { get; set; }

	public virtual User? User { get; set; }

    public virtual PaymentMethods PaymentMethod { get; set; }

    public DateTime OrderAt { get; set; }

    public DateTime DeliveryAt { get; set; }

	public double TotalPayment { get; set; }
}
