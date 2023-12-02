using Microsoft.AspNetCore.Mvc;
using MobileWeb.Areas.Admin.Services.ProductService;
using MobileWeb.Models;
using MobileWeb.Services.EmailService;
using System.Diagnostics;

namespace MobileWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _service;

        public HomeController(ILogger<HomeController> logger, IProductService service)
        {
            _logger = logger;
            _service = service;
        }

        public IActionResult Index()
        {
            var firstThreeProducts = _service.GetAllAsync()
                .OrderByDescending(p => p.Price)
                .Take(3);

            return View(firstThreeProducts);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //public async Task<IActionResult> SendMail()
        //{
        //    string to = "doantrihung4444@gmail.com";
        //    string subject = "TEST";
        //    string body = "<h1>Xin chào</h1>";

        //    await _service.SendMailAsync(to, subject, body);

        //    _logger.LogInformation("Email sended!");

        //    return NoContent();
        //}
    }
}