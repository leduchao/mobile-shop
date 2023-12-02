using Microsoft.AspNetCore.Mvc;
using MobileWeb.Areas.Admin.Services.OrderService;
using MobileWeb.Models.Entities;

namespace MobileWeb.Areas.Admin.Controllers;

[Area("Admin")]
[Route("admin/orders")]
public class OrdersController : Controller
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task<IActionResult> Index()
    {
        var orderList = await _orderService.GetAllOrdersAsync();
        return View(orderList);
    }

    [Route("order-detail")]
    public async Task<IActionResult> ShowOrderDetail(int oid)
    {
        var order = await _orderService.GetOrderByIdAsync(oid);

        order ??= new Order();

        ViewBag.OrderItems = order.OrderItems;

        return View(order);
    }

    [Route("delete-order")]
    public async Task<IActionResult> DeleteOrder(int oid)
    {
        var result = await _orderService.DeleteOrderAsync(oid);

        if (result)
        {
            return RedirectToAction(nameof(Index));
        }

        return View();
    }

    [Route("accept-order")]
    public async Task<IActionResult> AcceptOrder(int oid)
    {
        var result = await _orderService.AcceptOrderAsync(oid);

        if (result)
        {
            return RedirectToAction(nameof(Index));
        }

        return NotFound();
    }
}
