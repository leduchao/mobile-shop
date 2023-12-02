using Microsoft.AspNetCore.Mvc;

namespace MobileWeb.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
