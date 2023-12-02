using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MobileWeb.Models.Entities;

public class ProductColor
{
    [Key]
    public int Id { get; set; }

    public string ColorName { get; set; } = string.Empty;

    [ForeignKey("Product")]
    public int ProductId { get; set; }

    public Product Product { get; set; } = new();
}
