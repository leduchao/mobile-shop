using Microsoft.EntityFrameworkCore;
using MobileWeb.Data;
using MobileWeb.Models.DTOs;
using MobileWeb.Models.Entities;

namespace MobileWeb.Areas.Admin.Services.CategoryService
{
	public class CategoryService : ICategoryService
	{
		private readonly WebDbContext _context;

		public CategoryService(WebDbContext context)
		{
			_context = context;
		}

		public async Task CreateAsync(Category category)
		{
			_context.Categories.Add(category);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteAsync(Category category)
		{
			_context.Categories.Remove(category);
			await _context.SaveChangesAsync();
		}

		public async Task<List<Category>> GetAllAsync()
		{
			return await _context.Categories
				.Include(c => c.Products)
				.ToListAsync();
		}

		public async Task<Category?> GetByIdAsync(int id)
		{
			return await _context.Categories.FindAsync(id);
		}

		public async Task UpdateAsync(Category category)
		{
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }
    }
}
