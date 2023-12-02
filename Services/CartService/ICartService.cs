using MobileWeb.Models.DTOs;
using MobileWeb.Models.Entities;

namespace MobileWeb.Services.CartService;

public interface ICartService
{
    List<CartItem> GetAllItems();

    void ClearCart();

    void SaveCartSession(List<CartItem> cartItems);
}
