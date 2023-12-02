using Microsoft.EntityFrameworkCore;
using MobileWeb.Models.Entities;

namespace MobileWeb.Data;

public class WebDbContext : DbContext
{
    public WebDbContext(DbContextOptions<WebDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; }

    public DbSet<Product> Products { get; set; }

    public DbSet<Specifications> Specifications { get; set; }

    public DbSet<Order> Orders { get; set; }

    public DbSet<Cart> Carts { get; set; }
    
    public DbSet<CartItem> Items { get; set; }

    public DbSet<OrderItem> OrderItems { get; set; }

    public DbSet<ProductImage> ProductImages { get; set; }

    public DbSet<ProductColor> ProductColors { get; set; }
}
