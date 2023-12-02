using Microsoft.AspNetCore.Mvc;

namespace MobileWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Route("admin")]
	public class HomeController : Controller
	{
		//[Route("index")]
		public IActionResult Index()
		{
			return View();
		}
	}
}
