using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MobileWeb.Data;

namespace MobileWeb.Views.Shared.Components.FilterProducts
{  
    //[ViewComponent]
    public class FilterProducts : ViewComponent
    {
        private readonly WebDbContext _context;

        public FilterProducts(WebDbContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var listProducts = _context.Products!.Include(p => p.Category).ToList();
            return View(listProducts);
        }
    }
}
