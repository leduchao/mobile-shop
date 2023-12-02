using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MobileWeb.Areas.Identity.Services;
using MobileWeb.Models.DTOs;
using MobileWeb.Models.Entities;
using MobileWeb.Services.CartService;
using MobileWeb.Services.EmailService;
using MobileWeb.Services.UserService;
using System.Reflection.Metadata.Ecma335;

namespace MobileWeb.Controllers;

[Route("user")]
public class UserController : Controller
{
	private readonly ICartService _cartService;
	private readonly IUserService _userService;
	private readonly IEmailService _emailService;
	private readonly IAccountService _accountService;

	public UserController(ICartService cartService, IUserService userService, IEmailService emailService,
		IAccountService accountService)
	{
		_cartService = cartService;
		_userService = userService;
		_emailService = emailService;
		_accountService = accountService;
	}

	public async Task<IActionResult> Index(string uname)
	{
		//var user = await _userService.FindUserByIdAsync(uid);
		var user = await _userService.FindUserByNameAsync(uname);

		if (user is null)
			return NotFound();

		ViewBag.ProcessingOrderList = await _userService.GetProccessingOrdersAsync(user.Id);
		ViewBag.ShippingOrderList = await _userService.GetShippingOrdersAsync(user.Id);
		ViewBag.FinishedOrderList = await _userService.GetFinishedOrdersAsync(user.Id);

		return View(user);
	}

	[Route("edit-profile")]
	public async Task<IActionResult> EditProfile(string uname)
	{
		var user = await _userService.FindUserByNameAsync(uname);

		if (user is null)
			return NotFound();

		user.FirstName ??= "null";
		user.LastName ??= "null";
		user.Address ??= "null";
		user.Birthday ??= new DateTime(2023, 01, 01);
		user.Avatar ??= "avatar1.png";

		var userDTO = new UserDTO
		{
			UserName = user.UserName,
			FirstName = user.FirstName,
			LastName = user.LastName,
			Email = user.Email,
			Address = user.Address,
			PhoneNumber = user.PhoneNumber,
			Birthday = user.Birthday,
			//AvatarUrl = user.Avatar
		};

		ViewBag.UserId = user.Id;
		ViewBag.AvatarUrl = user.Avatar;

		return View(userDTO);
	}

	[HttpPost]
	[Route("edit-profile")]
	public async Task<IActionResult> EditProfile(string uid, UserDTO userDTO)
	{
		var result = await _userService.UpdateUserAsync(uid, userDTO);

		if (result)
			return RedirectToAction(nameof(Index), new { uname = userDTO.UserName });

		ModelState.AddModelError("UpdateUserError", "Không thể cập nhật thông tin! Vui lòng kiểm tra lại!");
		return View();
	}

	[Route("verify-email")]
	public async Task<IActionResult> VerifyEmail(string email)
	{
		var user = await _userService.FindUserByEmailAsync(email);

		if (user is null)
			return NotFound();

		var token = await _accountService.GenerateEmailConfirmTokenAsync(user);
		var link = Url.Action("ConfirmEmail", "User", new { uid = user.Id, token }, Request.Scheme);

		if (!string.IsNullOrEmpty(link))
		{
			await _emailService.SendMailAsync(user.Email, "Xác thực email", link);
			return RedirectToAction(nameof(ConfirmEmail));
		}

		ModelState.AddModelError("ConfirmFailed", "Không thể xác thực email!");
		return View();
	}

	[Route("confirm-email")]
	public IActionResult ConfirmEmail()
	{
		return View();
	}

	[HttpGet]
	[Route("confirm-email")]
	public async Task<IActionResult> ConfirmEmail(string uid, string token)
	{
		var result = await _accountService.ConfirmEmailAsync(uid, token);

		if (result)
			return RedirectToAction(nameof(Index), new { uid });

		ModelState.AddModelError("", "Không thể các thực email!");
		return View();
	}

	[Route("delete-account")]
	public async Task<IActionResult> DeleteUser(string uid)
	{
		var result = await _userService.DeleteUserAsync(uid);

		if (result)
		{
			await _accountService.LogoutAsync();
			return RedirectToAction("Index", "Home");
		}

		ModelState.AddModelError("", "Không thể xóa tài khoản!");
		return View();
	}

	//[HttpGet]
    [Route("change-password")]
    public IActionResult ChangePassword()//string uid)
    {
		//ViewBag.User = uid;
        return View();
    }

    [HttpPost]
	[Route("change-password")]
	public async Task<IActionResult> ChangePassword(ChangePasswordDto model)
	{
		if (ModelState.IsValid)
		{
			var result = await _userService.ChangePasswordAsync(model.UserName, model.CurrentPassword, model.NewPassword);

			if (result)
			{
				return RedirectToAction(nameof(Index), new { uname = model.UserName });
			}

            ModelState.AddModelError("", "Mật khẩu không chính xác!");
        }	

		ModelState.AddModelError("", "Không thể đổi mật khẩu!");
		return View();
	}
}