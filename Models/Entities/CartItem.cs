using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MobileWeb.Models.Entities;

public class CartItem
{
    [Key]
    public int Id { get; set; }

    public int Quantity { get; set; }

    public int ProductId { get; set; }

    public Product Product { get; set; } = new();

    public double TotalPrice => Product.Price * Quantity;

    public int CartId { get; set; }

    public Cart Cart { get; set; } = new();

}
