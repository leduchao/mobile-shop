using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using MobileWeb.Areas.Identity.Models.Account;
using MobileWeb.Models.Entities;
using MobileWeb.Services.EmailService;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace MobileWeb.Areas.Identity.Services;

public class AccountService : IAccountService
{
	private readonly UserManager<User> _userManager;
	private readonly SignInManager<User> _signinManager;
	private readonly RoleManager<IdentityRole> _roleManager;

	public AccountService(UserManager<User> userManager, SignInManager<User> signinManager,
		RoleManager<IdentityRole> roleManager)
	{
		_userManager = userManager;
		_signinManager = signinManager;
		_roleManager = roleManager;
	}

	public async Task<bool> LoginAsync(LoginViewModel model)
	{
		var user = await _userManager.FindByEmailAsync(model.Email);

		if (user is not null)
		{
			var result = await _signinManager.PasswordSignInAsync(
			user.UserName, model.Password,
			model.RememberMe, false);

			return result.Succeeded;
		}

		return false;
	}

	public async Task LogoutAsync()
	{
		await _signinManager.SignOutAsync();
	}

	public async Task<bool> RegisterAsync(RegisterViewModel model)
	{
		var existEmail = await _userManager.FindByEmailAsync(model.Email);
		var existUserName = await _userManager.FindByNameAsync(model.UserName);

		if (existEmail is not null || existUserName is not null)
		{
			return false;
		}

		var newUser = new User
		{
			UserName = model.UserName,
			Email = model.Email,
			CreatedAt = DateTime.Now,
			Avatar = "avatar1.png"
		};

		var result = await _userManager.CreateAsync(newUser, model.Password);

		if (result.Succeeded)
		{
			await AddRoleAsync(newUser);
		}

		return result.Succeeded;
	}

	private async Task AddRoleAsync(User user)
	{
		if (!await _roleManager.RoleExistsAsync("Admin"))
		{
			await _roleManager.CreateAsync(new IdentityRole("Admin"));
			await _userManager.AddToRoleAsync(user, "Admin");
		}
		else
		{
			var usersWithAdminRole = await _userManager.GetUsersInRoleAsync("Admin");

			if (usersWithAdminRole.Count == 0)
				await _userManager.AddToRoleAsync(user, "Admin");
			else
			{
				if (!await _roleManager.RoleExistsAsync("Customer"))
					await _roleManager.CreateAsync(new IdentityRole("Customer"));

				await _userManager.AddToRoleAsync(user, "Customer");
			}
		}
	}

	public async Task<bool> ConfirmEmailAsync(string uid, string token)
	{
		var user = await _userManager.FindByIdAsync(uid);

		if (user == null) return false;

		token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
		var result = await _userManager.ConfirmEmailAsync(user, token);

		return result.Succeeded;
	}

	public async Task<bool> ResetPasswordAsync(string uid, string token, string newPassword)
	{
		var user = await _userManager.FindByIdAsync(uid);

		if (user != null)
		{
			token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
			var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

			return result.Succeeded;
		}

		return false;
	}

	public async Task<User> GetUserByEmail(string email)
	{
		return await _userManager.FindByEmailAsync(email);
	}

	public async Task DeleteUserByEmailAsync(string email)
	{
		var user = await _userManager.FindByEmailAsync(email);

		if (user is not null)
			await _userManager.DeleteAsync(user);
	}

	public async Task<string> GenerateEmailConfirmTokenAsync(User user)
	{
		string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
		token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

		return token;
	}

	public async Task<string> GenerateForgotPaswordTokenAsync(User user)
	{
		string token = await _userManager.GeneratePasswordResetTokenAsync(user);
		token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

		return token;
	}

	public async Task<User> GetUserById(string id)
	{
		return await _userManager.FindByIdAsync(id);
	}
}
