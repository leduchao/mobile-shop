using Microsoft.EntityFrameworkCore;
using MobileWeb.Data;
using MobileWeb.Models.DTOs;
using MobileWeb.Models.Entities;

namespace MobileWeb.Areas.Admin.Services.ProductService;

public class ProductService : IProductService
{
	private readonly WebDbContext _context;

	public ProductService(WebDbContext context)
	{
		_context = context;
	}

	public async Task CreateAsync(ProductDTO request)
	{
		var specifications = new Specifications
		{
			RAM = request.RAM,
			ROM = request.ROM,
			NumberOfFrontCamera = 1,
			NumberOfBehindCamera = request.NumberOfBehindCamera,
			Model = request.Model,
			OperatingSystem = request.OperatingSystemVersion
		};

		var newProduct = new Product
		{
			Name = request.Name,
			Description = request.Description,
			Price = request.Price,
			Quantity = request.Quantity,
			Color = request.Color,
			CategoryId = request.CategoryId,
			Specifications = specifications,
			CreateAt = DateTime.Now
		};

		if (request.Avatar is not null)
		{
			newProduct.ImgUrl = SetFileName(request.Avatar);
			await UploadImageAssync(request.Avatar);
		}

		foreach (var image in request.Images)
		{
			var productImage = new ProductImage
			{
				ImageName = SetFileName(image),
				Product = newProduct
			};

			newProduct.Images.Add(productImage);
			await UploadImageAssync(image);
		}

		var cate = await _context.Categories
			.FirstOrDefaultAsync(c => c.Id == newProduct.CategoryId);

		if (cate is not null)
			specifications.Brand = cate.Name;
		else
            specifications.Brand = "No brand";

        _context.Products.Add(newProduct);
		await _context.SaveChangesAsync();
	}

	public async Task DeleteAsync(int id)
	{
		var product = await GetByIdAsync(id);

		if (product != null)
		{
			var productImages = _context.ProductImages.Where(pi => pi.ProductId == id);

			foreach (var image in productImages)
			{
				File.Delete(@"wwwroot\img\products\" + image.ImageName);
			}

			File.Delete(@"wwwroot\img\products\" + product.ImgUrl);

			_context.Products.Remove(product);
			await _context.SaveChangesAsync();
		}
	}

	public IQueryable<Product> GetAllAsync()
	{
		return _context.Products.Select(p => new Product
		{
			Id = p.Id,
			Name = p.Name,
			Description = p.Description,
			ImgUrl = p.ImgUrl,
			Price = p.Price,
			Quantity = p.Quantity,
			Category = p.Category,
			Specifications = p.Specifications,
			Color = p.Color,
			Colors = p.Colors,
			Images = p.Images,
			CreateAt = p.CreateAt
		});
	}

	public async Task<List<Category>> GetAllCategoriesAsync() => 
		await _context.Categories.ToListAsync();

	public async Task<Product?> GetByIdAsync(int id)
	{
		return await _context.Products
			.Include(p => p.Category)
			.Include(p => p.Specifications)
			.Include(p => p.Images)
			.FirstOrDefaultAsync(p => p.Id == id);
	}

	public async Task<Product?> GetByNameAsync(string productName)
	{
		return await _context.Products
			.Include(p => p.Category)
			.FirstOrDefaultAsync(p => p.Name == productName);
	}

	public async Task<Specifications?> GetSpecificationsAsync(int id)
	{
		return await _context.Specifications.FirstOrDefaultAsync(s => s.ProductId == id);
	}

	public async Task UpdateAsync(int id, ProductDTO request)
	{
		var existProduct = await GetByIdAsync(id);

		if (existProduct is not null)
		{
			var specifications = await _context.Specifications
				.FirstOrDefaultAsync(s => s.ProductId == id);

			// nếu có chỉnh sửa ảnh đại diện của sản phẩm
			if (request.AvatarUpdate is not null)
			{
				File.Delete(@"wwwroot\img\products\" + existProduct.ImgUrl);

				existProduct.ImgUrl = SetFileName(request.AvatarUpdate);
				await UploadImageAssync(request.AvatarUpdate);
			}

			if (specifications is not null)
			{
				specifications.RAM = request.RAM;
				specifications.ROM = request.ROM;
				specifications.NumberOfFrontCamera = 1;
				specifications.NumberOfBehindCamera = request.NumberOfBehindCamera;
				specifications.OperatingSystem = request.OperatingSystemVersion;
				specifications.Model = request.Model;

				existProduct.Name = request.Name;
				existProduct.Description = request.Description;
				existProduct.Price = request.Price;
				existProduct.Quantity = request.Quantity;
				existProduct.Color = request.Color;
				existProduct.CategoryId = request.CategoryId;

				var cate = _context.Categories
					.FirstOrDefault(c => c.Id == existProduct.CategoryId);
				
				if (cate is not null)
					specifications.Brand = cate.Name;

				existProduct.Specifications = specifications;
			}

			// nếu có chỉnh sửa các hình ảnh liên quan của sản phẩm
			if (request.Images is not null)
			{
				foreach (var image in request.Images)
				{
					var newImage = new ProductImage
					{
						ImageName = SetFileName(image),
						Product = existProduct
					};

					existProduct.Images.Add(newImage);
					await UploadImageAssync(image);
				}
			}    

			_context.Products.Update(existProduct);
			await _context.SaveChangesAsync();
		}
	}

	private static async Task UploadImageAssync(IFormFile file)
	{
		if (file != null)
		{
			var filePath = Path.Combine("wwwroot", "img", "products", SetFileName(file));

			using var fileStream = new FileStream(filePath, FileMode.Create);
			await file.CopyToAsync(fileStream);
		}
	}

	private static string SetFileName(IFormFile file) => 
		$"{DateTime.Now:yyyyMMddhhmmss}_{Path.GetFileName(file.FileName)}";

	public async Task<List<ProductImage>> GetAllProductImagesAsync(Product product)
	{
		var productImages = await _context.ProductImages
			.Where(pi => pi.ProductId == product.Id).ToListAsync();

		productImages ??= new List<ProductImage>();

		return productImages;
	}

	public async Task<bool> DeleteImageAsync(int id)
	{
		var productImage = await _context.ProductImages
			.FirstOrDefaultAsync(pi => pi.Id == id);

		if (productImage is not null)
		{
			_context.ProductImages.Remove(productImage);
			File.Delete(@"wwwroot\img\products\" + productImage.ImageName);

			await _context.SaveChangesAsync();

			return true;
		}

		return false;
	}
}
