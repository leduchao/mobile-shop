using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MobileWeb.Data;
using MobileWeb.Models.Entities;
using MobileWeb.Services.CartService;

namespace MobileWeb.Controllers
{
    public class ProductsController : Controller
    {
        private readonly WebDbContext _context;
        private readonly ICartService _cartService;

        public ProductsController(WebDbContext context, ICartService cartService)
        {
            _context = context;
            _cartService = cartService;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var mobileWebContext = _context?.Products?.Include(p => p.Category);

            //HttpContext.Session.SetString("name", "current_user_name");
            //var session = HttpContext.Session.GetString("name");

            return View(await mobileWebContext!.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }       

        private bool ProductExists(int id)
        {
            return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [Route("/cartt", Name = "cartt")]
        public IActionResult Cart()
        {
            return View(_cartService.GetAllItems());
        }

        [Authorize(Roles = "user")]
        [Route("/cartt/{product-id:int}", Name = "AddCart")]
        public IActionResult AddToCart(int productId)
        {
            if (!User.Identity!.IsAuthenticated)
                return RedirectToAction("Login", "Users");

            var product = _context.Products?.Where(p => p.Id == productId)
                                          .FirstOrDefault();
            if (product == null)
            {
                return NotFound();
            }
            var cart = _cartService.GetAllItems();
            var cartitem = cart?.Find(p => p.Product?.Id == productId);
            if (cartitem != null)
            {
                // Đã tồn tại, tăng thêm 1
                cartitem.Quantity++;
            }
            else
            {
                //  Thêm mới
                cart?.Add(new CartItem() { Quantity = 1, Product = product });
            }

            // Lưu cart vào Session
            _cartService.SaveCartSession(cart!);
            return RedirectToAction(nameof(Cart));
        }

        //cập nhật ljai thông tin của sản phẩm trong giỏ
        [HttpPost]
        [Route("/updatecart", Name = "updatecart")]
        public IActionResult UpdateCart([FromForm] int productid, [FromForm] int quantity)
        {
            // Cập nhật Cart thay đổi số lượng quantity ...
            var cart = _cartService.GetAllItems();
            var cartitem = cart?.Find(p => p.Product?.Id == productid);
            if (cartitem != null)
            {
                // Đã tồn tại, tăng thêm 1
                cartitem.Quantity = quantity;
            }
            _cartService.SaveCartSession(cart!);
            // Trả về mã thành công (không có nội dung gì - chỉ để Ajax gọi)
            return Ok();
        }

        //xóa 1 sản phẩm ra khỏi giỏ
        [Route("/removecart/{productid:int}", Name = "removecart")]
        public IActionResult RemoveCart([FromRoute] int productid)
        {
            var cart = _cartService.GetAllItems();
            var cartitem = cart?.Find(p => p.Product?.Id == productid);
            if (cartitem != null)
            {
                // Đã tồn tại, giảm đi 1
                cart?.Remove(cartitem);
            }

            _cartService.SaveCartSession(cart!);
            return RedirectToAction(nameof(Cart));
        }
    }
}
