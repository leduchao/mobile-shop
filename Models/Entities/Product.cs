using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MobileWeb.Models.Entities;

public class Product
{
    [Key]
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? ImgUrl { get; set; }

    public double Price { get; set; }

    public int Quantity { get; set; } // số lượng tồn kho

    [ForeignKey("Category")]
    public int CategoryId { get; set; }

    public Category? Category { get; set; }

    public string? Color { get; set; }

    public Specifications? Specifications { get; set; } // thong so chi tiet

    public List<ProductImage> Images { get; set; } = new();

    public List<ProductColor> Colors { get; set; } = new();

    public DateTime CreateAt { get; set; }
}
