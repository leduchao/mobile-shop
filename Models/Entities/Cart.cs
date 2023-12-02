using System.ComponentModel.DataAnnotations;

namespace MobileWeb.Models.Entities;

public class Cart
{
    [Key]
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public User User { get; set; } = new();

    public List<CartItem> Items { get; set; } = new();

    //public double TotalPrice { get; set; }
}