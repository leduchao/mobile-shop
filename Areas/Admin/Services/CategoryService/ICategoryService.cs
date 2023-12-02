using MobileWeb.Models.DTOs;
using MobileWeb.Models.Entities;

namespace MobileWeb.Areas.Admin.Services.CategoryService
{
    public interface ICategoryService
    {
        Task<List<Category>> GetAllAsync();

        Task<Category?> GetByIdAsync(int id);

        Task CreateAsync(Category category);

        Task UpdateAsync(Category category);

        Task DeleteAsync(Category category);
    }
}
