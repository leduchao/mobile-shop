using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MobileWeb.Models.Entities;

public class ProductImage
{
	[Key]
	public int Id { get; set; }

	public string ImageName { get; set; } = string.Empty;

	[ForeignKey("Product")]
	public int ProductId { get; set; }

	public Product Product { get; set; } = new();

}
