using MobileWeb.Models.Entities;

namespace MobileWeb.Areas.Admin.Services.OrderService;

public interface IOrderService
{
	Task<List<Order>> GetAllOrdersAsync();

	Task<Order?> GetOrderByIdAsync(int oid);

	Task<bool> DeleteOrderAsync(int oid);

	Task<bool> AcceptOrderAsync(int oid);
}
