using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MobileWeb.Data;
using MobileWeb.Models.DTOs;
using MobileWeb.Models.Entities;

namespace MobileWeb.Areas.Admin.Services.OrderService;

public class OrderService : IOrderService
{
    private readonly WebDbContext _context;
    private readonly UserManager<User> _userManager;

    public OrderService(WebDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<bool> AcceptOrderAsync(int oid)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == oid);

        if (order is not null)
        {
            order.Status = Status.Shipping;
            await _context.SaveChangesAsync();

            return true;
        }

        return false;
    }

    public async Task<bool> DeleteOrderAsync(int oid)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == oid);

        if (order is not null)
        {
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public async Task<List<Order>> GetAllOrdersAsync()
    {
        var orderList = await _context.Orders
            .OrderBy(o => o.Status)
            .ToListAsync();

        orderList ??= new List<Order>();

        foreach (var order in orderList)
        {
            var user = await _userManager.FindByIdAsync(order.UserId);

            if (user is not null)
            {
                order.User = user;
            }
        }

        return orderList;
    }

    public async Task<Order?> GetOrderByIdAsync(int oid)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == oid);

        if (order is not null)
        {
            var orderItemList = await _context.OrderItems
                .Where(oi => oi.OrderId == order.Id)
                .ToListAsync();

            if (orderItemList is not null && orderItemList.Count != 0)
            {
                foreach (var item in orderItemList)
                {
                    var product = await _context.Products
                        .FirstOrDefaultAsync(p => p.Id == item.ProductId);

                    if (product is not null)
                        item.Product = product;
                }

                order.OrderItems = orderItemList;
            }
        }

        return order;
    }
}
