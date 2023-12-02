using MobileWeb.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace MobileWeb.Models.DTOs
{
    public class CategoryDTO
    {
        [Required(ErrorMessage = "Không được bỏ trống tên danh mục!")]
        [Display(Name = "Tên danh mục")]
        public string? Name { get; set; }

        public List<Product>? Products { get; set; }
    }
}
