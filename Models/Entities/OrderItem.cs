using MobileWeb.Models.DTOs;
using System.ComponentModel.DataAnnotations;

namespace MobileWeb.Models.Entities;

public class OrderItem
{
	[Key]
	public int Id { get; set; }

	public int OrderId { get; set; }

	public Order? Order { get; set; }

	public int ProductId { get; set; }

	public Product? Product { get; set; }

	public int Quantity { get; set; }

	public double UnitPrice { get; set; }

	public double TotalPrice => UnitPrice * Quantity;
}
