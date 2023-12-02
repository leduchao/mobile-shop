using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using MobileWeb.Data;
using Microsoft.AspNetCore.Identity;
using MobileWeb.Models.Entities;
using MobileWeb.Areas.Admin.Services.CategoryService;
using MobileWeb.Areas.Admin.Services.ProductService;
using MobileWeb.Areas.Identity.Services;
using MobileWeb.Services.EmailService;
using MobileWeb.Services.CartService;
using MobileWeb.Services.UserService;
using MobileWeb.Areas.Admin.Services.OrderService;
//using MobileWeb.Sqlite;

var builder = WebApplication.CreateBuilder(args);

//add database context
builder.Services.AddDbContext<WebDbContext>(options =>
{
	options.UseSqlServer(builder.Configuration.GetConnectionString("Database"));
});

// them data context cho identity
builder.Services.AddDbContext<UserDbContext>(options =>
{
	options.UseSqlServer(builder.Configuration.GetConnectionString("Database"));
});

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
	options.Password.RequireNonAlphanumeric = false; // ky tu dac biet
	options.Password.RequireDigit = false; // ky tu so
	options.Password.RequireUppercase = false; // ky tu in hoa
	options.Password.RequireLowercase = false; // ky tu thuong
	options.SignIn.RequireConfirmedAccount = false; // phai xac thuc tai khoan
})
	.AddEntityFrameworkStores<UserDbContext>()
	.AddDefaultTokenProviders();

// them cookies cho xac thuc
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
				.AddCookie();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(30);
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
});

builder.Services.ConfigureApplicationCookie(options =>
{
	options.AccessDeniedPath = "/identity/account/access-denied";
	options.LoginPath = "/identity/account/login";
	options.LogoutPath = "/identity/account/logout";
});

// login, signup, reset password
builder.Services.AddScoped<IAccountService, AccountService>();

// dang ky dich vu thao tac voi gio hang
builder.Services.AddTransient<ICartService, CartService>();

builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IOrderService, OrderService>();

// mail settings
builder.Services.AddOptions();
var mailSettingsSection = builder.Configuration.GetSection("MailSettings");
builder.Services.Configure<MailSettings>(mailSettingsSection);
	
builder.Services.AddTransient<IEmailService, EmailService>();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
  name: "area",
  pattern: "{area:exists}/{controller}/{action}/{id?}");

app.MapControllerRoute(
  name: "default",
  pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
