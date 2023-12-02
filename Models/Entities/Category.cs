using System.ComponentModel.DataAnnotations;

namespace MobileWeb.Models.Entities
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        public string? Name { get; set; }

        public List<Product>? Products { get; set; } // mot danh muc co nhieu san pham (quan he 1-n)
    }
}
