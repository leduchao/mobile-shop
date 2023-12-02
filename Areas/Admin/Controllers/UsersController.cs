
using Microsoft.AspNetCore.Mvc;
using MobileWeb.Services.UserService;

namespace MobileWeb.Areas.Admin.Controllers;

[Area("Admin")]
[Route("users")]
public class UsersController : Controller
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<IActionResult> Index(string name)
    {
        var users = await _userService.GetAllUsersAsync();
        //name = name.ToLower();

        if (!string.IsNullOrEmpty(name))
        {
            users = users.Where(u => u.UserName.Contains(name.ToLower()) || 
                u.FirstName.ToLower().Contains(name.ToLower()) || 
                u.LastName.ToLower().Contains(name.ToLower()))
                .ToList();
        }

        return View(users);
    }

    [Route("detail")]
    public async Task<IActionResult> Details(string uid)
    {
        var user = await _userService.FindUserByIdAsync(uid);

        return View(user);
    }

    [Route("delete-user")]
    public async Task<IActionResult> DeleteUser(string uid)
    {
        var result = await _userService.DeleteUserAsync(uid);

        if (result)
            return RedirectToAction(nameof(Index));

        return View();
    }
}
