using MobileWeb.Data;
using MobileWeb.Models.DTOs;
using MobileWeb.Models.Entities;
using Newtonsoft.Json;

namespace MobileWeb.Services.CartService;

public class CartService : ICartService
{
    public const string CARTKEY = "cart";
    private readonly IHttpContextAccessor _context;
    private readonly HttpContext _httpContext;

    public CartService(IHttpContextAccessor context)
    {
        _context = context;
        _httpContext = context.HttpContext!;
    }

    // Lấy cart từ Session (danh sách CartItem)
    public List<CartItem> GetAllItems()
    {
        var session = _httpContext.Session;
        string? jsonCart = session.GetString(CARTKEY);

        if (!string.IsNullOrEmpty(jsonCart))
        {

            var cartItems = JsonConvert.DeserializeObject<List<CartItem>>(jsonCart);

            if (cartItems != null)
                return cartItems;
        }

        return new List<CartItem>();
    }

    // Xóa cart khỏi session
    public void ClearCart()
    {
        var session = _httpContext.Session;
        session.Remove(CARTKEY);
    }

    // Lưu Cart (Danh sách CartItem) vào session
    public void SaveCartSession(List<CartItem> cartItems)
    {
        var session = _httpContext.Session;

        string jsoncart = JsonConvert.SerializeObject(cartItems, new JsonSerializerSettings()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        });

        session.SetString(CARTKEY, jsoncart);
    }
}
