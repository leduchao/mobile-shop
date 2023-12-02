using Microsoft.AspNetCore.Mvc;

namespace MobileWeb.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
