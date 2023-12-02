using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using MobileWeb.Services.CartService;

namespace MobileWeb.Controllers;

[Route("cart")]
public class CartController : Controller
{
	private readonly ICartService _cartService;

	public CartController(ICartService cartService)
	{
		_cartService = cartService;
	}

	public IActionResult Index()
	{
		return View(_cartService.GetAllItems());
	}

	[Route("clear")]
	public IActionResult ClearCart()
	{
		_cartService.ClearCart();
		return RedirectToAction(nameof(Index));
	}

	[Route("remove-one")]
	public IActionResult RemoveItemById(int id)
	{
		var cartItems = _cartService.GetAllItems();

		var item = cartItems.FirstOrDefault(i => i.Product.Id == id);

		if (item != null)
		{
			cartItems.Remove(item);
			_cartService.SaveCartSession(cartItems);
		}

		return RedirectToAction(nameof(Index));
	}

    [HttpPost]
    [Route("update-cart")]
    public IActionResult UpdateCart(int pid, int quantity)
    {
        var cart = _cartService.GetAllItems();

        var cartitem = cart.FirstOrDefault(p => p.Product.Id == pid);

        if (cartitem != null)
		{
			if (cartitem.Quantity < 1)
			{
                cartitem.Quantity = 1;
            }
			else
				cartitem.Quantity = quantity;
		}

        _cartService.SaveCartSession(cart);
        
        return RedirectToAction(nameof(Index));
    }
}
