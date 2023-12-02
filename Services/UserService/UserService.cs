using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using MobileWeb.Areas.Admin.Services.ProductService;
using MobileWeb.Areas.Identity.Services;
using MobileWeb.Data;
using MobileWeb.Models.DTOs;
using MobileWeb.Models.Entities;
using MobileWeb.Services.CartService;
using MobileWeb.Services.EmailService;
using System.Text;

namespace MobileWeb.Services.UserService;

public class UserService : IUserService
{
    private readonly WebDbContext _dbContext;
    private readonly UserDbContext _userDbContext;
    private readonly ICartService _cartService;
    private readonly UserManager<User> _userManager;
    private readonly IEmailService _emailService;

    public UserService(WebDbContext dbContext, UserDbContext userDbContext,
        ICartService cartService, UserManager<User> userManager, IEmailService emailService)
    {
        _dbContext = dbContext;
        _userDbContext = userDbContext;
        _cartService = cartService;
        _userManager = userManager;
        _emailService = emailService;
    }

    public async Task<User> FindUserByIdAsync(string uid)
    {
        return await _userManager.FindByIdAsync(uid);
    }

    public async Task<User> FindUserByNameAsync(string uname)
    {
        return await _userManager.FindByNameAsync(uname);
    }

    public async Task<User> FindUserByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<bool> Order(string uid, OrderDTO orderDTO)
    {
        var cart = _cartService.GetAllItems();
        var user = await _userManager.FindByIdAsync(uid);

        if (user != null)
        {
            try
            {
                user.FirstName = orderDTO.Firstname;
                user.LastName = orderDTO.LastName;
                user.Address = orderDTO.Address;
                user.PhoneNumber = orderDTO.PhoneNumber;

                var newOrder = new Order
                {
                    Status = Status.Processing,
                    UserId = user.Id,
                    PaymentMethod = orderDTO.PaymentMethod,
                    OrderAt = DateTime.Now,
                    DeliveryAt = DateTime.Now.AddDays(4d)
                };

                foreach (var item in cart)
                {
                    var newOrderItem = new OrderItem
                    {
                        ProductId = item.Product.Id,
                        Quantity = item.Quantity,
                        UnitPrice = item.Product.Price,
                        Order = newOrder
                    };

                    newOrder.OrderItems.Add(newOrderItem);
                    newOrder.TotalPayment += newOrderItem.UnitPrice * newOrderItem.Quantity;
                }

                await _userManager.UpdateAsync(user);
                _dbContext.Orders.Add(newOrder);

                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        return false;
    }

    public async Task<bool> UpdateUserAsync(string uid, UserDTO userDTO)
    {
        var user = await _userManager.FindByIdAsync(uid);

        if (user is null)
            return false;

        user.UserName = userDTO.UserName;
        //user.Email = userDTO.Email;
        user.FirstName = userDTO.FirstName;
        user.LastName = userDTO.LastName;
        user.Address = userDTO.Address;
        user.Birthday = userDTO.Birthday;
        user.PhoneNumber = userDTO.PhoneNumber;
        user.Birthday = userDTO.Birthday;

        if (userDTO.Avatar != null)
        {
            var oldImg = user.Avatar;

            if (oldImg is not null)
                File.Delete(@"wwwroot\img\users\" + oldImg);

            user.Avatar = SetFileName(userDTO.Avatar);
            await UploadImageAssync(userDTO.Avatar);
        }

        await _userManager.UpdateAsync(user);
        await _userDbContext.SaveChangesAsync();

        return true;
    }

    private static string SetFileName(IFormFile file)
        => $"{DateTime.Now:yyyyMMddhhmmss}_{Path.GetFileName(file.FileName)}";

    private static async Task UploadImageAssync(IFormFile file)
    {
        if (file != null)
        {
            var filePath = Path.Combine("wwwroot", "img", "users", SetFileName(file));

            using var fileStream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(fileStream);
        }
    }

    public async Task<bool> VerifyEmailAsync(User user)
    {
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        await _emailService.SendMailAsync(user.Email, "Xác thực email", token);

        return true;
    }

    public async Task<bool> DeleteUserAsync(string uid)
    {
        var user = await _userManager.FindByIdAsync(uid);

        if (user is not null)
        {
            if (!string.IsNullOrEmpty(user.Avatar))
                File.Delete(@"wwwroot\img\users\" + user.Avatar);

            var userOrderList = await _dbContext.Orders
                .Where(o => o.UserId == user.Id)
                .ToListAsync();

            if (userOrderList is not null && userOrderList.Count != 0)
            {
                foreach (var order in userOrderList)
                {
                    _dbContext.Orders.Remove(order);
                }

                await _dbContext.SaveChangesAsync();
            }

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }

        return false;
    }

    public async Task<string> GeneratePasswordChangeTokenAsync(User user)
    {
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        return token;
    }

    public async Task<bool> ChangePasswordAsync(string uname, string currentPassword, string newPassword)
    {
        var user = await _userManager.FindByNameAsync(uname);

        if (user is null)
            return false;

        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        return result.Succeeded;
    }

    public async Task<List<Order>> GetProccessingOrdersAsync(string uid)
    {
        var orders = await _dbContext.Orders
            .Where(o => o.UserId == uid && o.Status == Status.Processing)
            .ToListAsync();

        if (orders is not null && orders.Count != 0)
        {
            foreach (var order in orders)
            {
                var orderItemList = await _dbContext.OrderItems
                    .Where(oi => oi.OrderId == order.Id).ToListAsync();

                foreach(var item in orderItemList)
                {
                    var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId);

                    if (product is not null)
                        item.Product = product;
                }                    

                order.OrderItems = orderItemList;
            }

            return orders;
        }

        return new List<Order>();
    }

    public async Task<List<Order>> GetShippingOrdersAsync(string uid)
    {
        var orders = await _dbContext.Orders
            .Where(o => o.UserId == uid && o.Status == Status.Shipping)
            .ToListAsync();

        if (orders is not null && orders.Count != 0)
        {
            foreach (var order in orders)
            {
                var orderItemList = await _dbContext.OrderItems
                    .Where(oi => oi.OrderId == order.Id).ToListAsync();

                foreach (var item in orderItemList)
                {
                    var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId);

                    if (product is not null)
                        item.Product = product;
                }

                order.OrderItems = orderItemList;
            }

            return orders;
        }

        return new List<Order>();
    }

    public async Task<List<Order>> GetFinishedOrdersAsync(string uid)
    {
        var orders = await _dbContext.Orders
            .Where(o => o.UserId == uid && o.Status == Status.Finished)
            .ToListAsync();

        if (orders is not null && orders.Count != 0)
        {
            foreach (var order in orders)
            {
                var orderItemList = await _dbContext.OrderItems
                    .Where(oi => oi.OrderId == order.Id).ToListAsync();

                foreach (var item in orderItemList)
                {
                    var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId);

                    if (product is not null)
                        item.Product = product;
                }

                order.OrderItems = orderItemList;
            }

            return orders;
        }

        return new List<Order>();
    }

    public async Task<IList<User>> GetAllUsersAsync()
    {
        var userList = await _userManager.GetUsersInRoleAsync("Customer");

        if (userList is not null)
            return userList;

        return new List<User>();
    }
}
