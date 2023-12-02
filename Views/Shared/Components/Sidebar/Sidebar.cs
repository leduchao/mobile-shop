using Microsoft.AspNetCore.Mvc;
using MobileWeb.Data;

namespace MobileWeb.Views.Shared.Components.Sidebar;

public class Sidebar : ViewComponent
{
    private readonly WebDbContext _context;

    public Sidebar(WebDbContext context)
    {
        _context = context;
    }

    public IViewComponentResult Invoke()
    {
        var categories = _context.Categories.ToList();
        return View(categories);
    }
}
