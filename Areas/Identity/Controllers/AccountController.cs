
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MobileWeb.Areas.Identity.Models.Account;
using MobileWeb.Areas.Identity.Services;
using MobileWeb.Models.Entities;
using MobileWeb.Services.EmailService;
using NuGet.Common;

namespace App.Areas.Identity.Controllers;

[Area("Identity")]
[Route("identity/account")]
public class AccountController : Controller
{
	private readonly IAccountService _accountService;
	private readonly IEmailService _emailService;
	private readonly ILogger<AccountController> _logger;

	public AccountController(IAccountService accountService, IEmailService emailService,
        ILogger<AccountController> logger)
	{
		_accountService = accountService;
		_emailService = emailService;
		_logger = logger;
	}

	[Route("login")]
	public async Task<IActionResult> Login()
	{
		await _accountService.LogoutAsync();
		return View();
	}

	// POST: identity/account/login
	[HttpPost]
	[Route("login")]
	public async Task<IActionResult> Login(LoginViewModel model)
	{
		if (ModelState.IsValid)
		{
			var result = await _accountService.LoginAsync(model);

			if (result)
			{
				_logger.LogInformation("User has logged in!");
				return RedirectToAction("Index", "Home", new { area = "" });
			}
			else
			{
				ModelState.AddModelError("", "Email hoặc mật khẩu không đúng!");
				return View(model);
			}
		}

		return View(model);
	}

	[Route("logout")]
	public async Task<IActionResult> Logout()
	{
		await _accountService.LogoutAsync();
		_logger.LogInformation("User has logged out!");

		return RedirectToAction("Index", "Home", new { area = "" });
	}
	
	// GET: /Account/Register
	[Route("register")]
	public async Task<IActionResult> Register()
	{
		await _accountService.LogoutAsync();
		return View();
	}

	// POST: /Account/Register
	[HttpPost]
	[Route("register")]
	public async Task<IActionResult> Register(RegisterViewModel model)
	{
		if (ModelState.IsValid)
		{
			var result = await _accountService.RegisterAsync(model);

			if (result)
			{
				var user = await _accountService.GetUserByEmail(model.Email!);
				_logger.LogInformation("User has registed!");

				var token = await _accountService.GenerateEmailConfirmTokenAsync(user);
				var callbackUrl = Url.Action("ConfirmEmail", "Account", new { uid = user.Id, token }, Request.Scheme);

				if (callbackUrl is not null)
				{
					string subject = "Xác thực email";
					string body = $"Hãy <a href='{callbackUrl}'>click vào đây</a> để xác thực email!";

					await _emailService.SendMailAsync(model.Email!, subject, body);

					return RedirectToAction(nameof(ConfirmEmail));
				}
				else
					await _accountService.DeleteUserByEmailAsync(model.Email!);
			}
			else
			{
				ModelState.AddModelError("", "Email đã được đăng ký hoặc tên tài khoản đã được sử dụng!");
			}

		}

		return View(model);
	}

	// verify email
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
			return RedirectToAction(nameof(Login));

		return View();
	}

	// forgot password
	[Route("forgot-password")]
	public IActionResult ForgotPassword()
	{
		return View();
	}

	[HttpPost]
	[Route("forgot-password")]
	public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
	{
		var user = await _accountService.GetUserByEmail(model.Email!);

		if (user != null)
		{
			var callbackUrl = Url.Action("ResetPassword", "Account", new { uid = user.Id }, Request.Scheme);

			string subject = "Đặt lại mật khẩu";
			string body = $"Hãy <a href='{callbackUrl}'>click vào đây</a> để đặt lại mật khẩu!";

			await _emailService.SendMailAsync(model.Email!, subject, body);
			_logger.LogInformation("Email has been sended!");

			return RedirectToAction(nameof(ConfirmResetPassword));
		}

        ModelState.AddModelError("", "Email không chính xác!");
        return View();
	}

	// confirm reset pass
	[Route("confirm-reset-password")]
	public IActionResult ConfirmResetPassword()
	{
		return View();
	}

	// reset pass
	[HttpGet]
	[Route("reset-password")]
	public IActionResult ResetPassword(string uid)
	{
		ViewBag.UserId = uid;
		return View();
	}

	[HttpPost]
	[Route("reset-password")]
	public async Task<IActionResult> ResetPassword(string uid, ResetPasswordViewModel model)
	{
		var user = await _accountService.GetUserById(uid);

		if (user != null)
		{
			var token = await _accountService.GenerateForgotPaswordTokenAsync(user);

			if (string.IsNullOrEmpty(model.Password))
				model.Password = "passwordnull";
			else
			{
				var result = await _accountService.ResetPasswordAsync(uid, token, model.Password);

				if (result)
				{
					_logger.LogInformation("Password changed successfully!");
					return RedirectToAction(nameof(Login));
				}
			}
		}

		return View();
	}

	//
	// POST: /Account/ExternalLogin
	//[HttpPost]
	//[AllowAnonymous]
	//[ValidateAntiForgeryToken]
	//public IActionResult ExternalLogin(string provider, string? returnUrl = null)
	//{
	//    returnUrl ??= Url.Content("~/");
	//    var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
	//    var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
	//    return Challenge(properties, provider);
	//}
	////
	//// GET: /Account/ExternalLoginCallback
	//[HttpGet]
	//[AllowAnonymous]
	//public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
	//{
	//    returnUrl ??= Url.Content("~/");
	//    if (remoteError != null)
	//    {
	//        ModelState.AddModelError(string.Empty, $"Lỗi sử dụng dịch vụ ngoài: {remoteError}");
	//        return View(nameof(Login));
	//    }
	//    var info = await _signInManager.GetExternalLoginInfoAsync();
	//    if (info == null)
	//    {
	//        return RedirectToAction(nameof(Login));
	//    }

	//    // Sign in the user with this external login provider if the user already has a login.
	//    var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
	//    if (result.Succeeded)
	//    {
	//        // Cập nhật lại token
	//        await _signInManager.UpdateExternalAuthenticationTokensAsync(info);

	//        _logger.LogInformation(5, "User logged in with {Name} provider.", info.LoginProvider);
	//        return LocalRedirect(returnUrl);
	//    }
	//    if (result.RequiresTwoFactor)
	//    {
	//        return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl });
	//    }
	//    if (result.IsLockedOut)
	//    {
	//        return View("Lockout");
	//    }
	//    else
	//    {
	//        // If the user does not have an account, then ask the user to create an account.
	//        ViewData["ReturnUrl"] = returnUrl;
	//        ViewData["ProviderDisplayName"] = info.ProviderDisplayName;
	//        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
	//        return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = email });
	//    }
	//}

	///* external login confirmation
	//// POST: /Account/ExternalLoginConfirmation
	//[HttpPost]
	//[AllowAnonymous]
	//[ValidateAntiForgeryToken]
	//public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl = null)
	//{
	//    returnUrl ??= Url.Content("~/");
	//    if (ModelState.IsValid)
	//    {
	//        // Get the information about the user from the external login provider
	//        var info = await _signInManager.GetExternalLoginInfoAsync();
	//        if (info == null)
	//        {
	//            return View("ExternalLoginFailure");
	//        }

	//        // Input.Email
	//        var registeredUser = await _userManager.FindByEmailAsync(model.Email);
	//        string externalEmail = null;
	//        AppUser externalEmailUser = null;

	//        // Claim ~ Dac tinh mo ta mot doi tuong 
	//        if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
	//        {
	//            externalEmail = info.Principal.FindFirstValue(ClaimTypes.Email);
	//        }

	//        if (externalEmail != null)
	//        {
	//            externalEmailUser = await _userManager.FindByEmailAsync(externalEmail);
	//        }

	//        if ((registeredUser != null) && (externalEmailUser != null))
	//        {
	//            // externalEmail  == Input.Email
	//            if (registeredUser.Id == externalEmailUser.Id)
	//            {
	//                // Lien ket tai khoan, dang nhap
	//                var resultLink = await _userManager.AddLoginAsync(registeredUser, info);
	//                if (resultLink.Succeeded)
	//                {
	//                    await _signInManager.SignInAsync(registeredUser, isPersistent: false);
	//                    return LocalRedirect(returnUrl);
	//                }
	//            }
	//            else
	//            {
	//                // registeredUser = externalEmailUser (externalEmail != Input.Email)

	//                    info => user1 (mail1@abc.com)
	//                         => user2 (mail2@abc.com)

	//                ModelState.AddModelError(string.Empty, "Không liên kết được tài khoản, hãy sử dụng email khác");
	//                return View();
	//            }
	//        }


	//        if ((externalEmailUser != null) && (registeredUser == null))
	//        {
	//            ModelState.AddModelError(string.Empty, "Không hỗ trợ tạo tài khoản mới - có email khác email từ dịch vụ ngoài");
	//            return View();
	//        }

	//        if ((externalEmailUser == null) && (externalEmail == model.Email))
	//        {
	//            // Chua co Account -> Tao Account, lien ket, dang nhap
	//            var newUser = new AppUser()
	//            {
	//                UserName = externalEmail,
	//                Email = externalEmail
	//            };

	//            var resultNewUser = await _userManager.CreateAsync(newUser);
	//            if (resultNewUser.Succeeded)
	//            {
	//                await _userManager.AddLoginAsync(newUser, info);
	//                var code = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
	//                await _userManager.ConfirmEmailAsync(newUser, code);

	//                await _signInManager.SignInAsync(newUser, isPersistent: false);

	//                return LocalRedirect(returnUrl);

	//            }
	//            else
	//            {
	//                ModelState.AddModelError("Không tạo được tài khoản mới");
	//                return View();
	//            }
	//        }


	//        var user = new AppUser { UserName = model.Email, Email = model.Email };
	//        var result = await _userManager.CreateAsync(user);
	//        if (result.Succeeded)
	//        {
	//            result = await _userManager.AddLoginAsync(user, info);
	//            if (result.Succeeded)
	//            {
	//                await _signInManager.SignInAsync(user, isPersistent: false);
	//                _logger.LogInformation(6, "User created an account using {Name} provider.", info.LoginProvider);

	//                // Update any authentication tokens as well
	//                await _signInManager.UpdateExternalAuthenticationTokensAsync(info);

	//                return LocalRedirect(returnUrl);
	//            }
	//        }
	//        ModelState.AddModelError(result);
	//    }

	//    ViewData["ReturnUrl"] = returnUrl;
	//    return View(model);
	//} */

	///* send code
	////POST: /Account/SendCode
	////[HttpPost]
	////[AllowAnonymous]
	////[ValidateAntiForgeryToken]
	////public async Task<IActionResult> SendCode(SendCodeViewModel model)
	////{
	////    if (!ModelState.IsValid)
	////    {
	////        return View();
	////    }

	////    var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
	////    if (user == null)
	////    {
	////        return View("Error");
	////    }
	////    // Dùng mã Authenticator
	////    if (model.SelectedProvider == "Authenticator")
	////    {
	////        return RedirectToAction(nameof(VerifyAuthenticatorCode), new { ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
	////    }

	////    // Generate the token and send it
	////    var code = await _userManager.GenerateTwoFactorTokenAsync(user, model.SelectedProvider);
	////    if (string.IsNullOrWhiteSpace(code))
	////    {
	////        return View("Error");
	////    }

	////    var message = "Your security code is: " + code;
	////    if (model.SelectedProvider == "Email")
	////    {
	////        await _emailSender.SendEmailAsync(await _userManager.GetEmailAsync(user), "Security Code", message);
	////    }
	////    else if (model.SelectedProvider == "Phone")
	////    {
	////        await _emailSender.SendSmsAsync(await _userManager.GetPhoneNumberAsync(user), message);
	////    }

	////    return RedirectToAction(nameof(VerifyCode), new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
	////}
	//*/

	//// GET: /Account/VerifyCode
	//[HttpGet]
	//[AllowAnonymous]
	//public async Task<IActionResult> VerifyCode(string provider, bool rememberMe, string returnUrl = null)
	//{
	//    // Require that the user has already logged in via username/password or external login
	//    var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
	//    if (user == null)
	//    {
	//        return View("Error");
	//    }
	//    return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
	//}

	////
	//// POST: /Account/VerifyCode
	//[HttpPost]
	//[AllowAnonymous]
	//[ValidateAntiForgeryToken]
	//public async Task<IActionResult> VerifyCode(VerifyCodeViewModel model)
	//{
	//    model.ReturnUrl ??= Url.Content("~/");
	//    if (!ModelState.IsValid)
	//    {
	//        return View(model);
	//    }

	//    // The following code protects for brute force attacks against the two factor codes.
	//    // If a user enters incorrect codes for a specified amount of time then the user account
	//    // will be locked out for a specified amount of time.
	//    var result = await _signInManager.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe, model.RememberBrowser);
	//    if (result.Succeeded)
	//    {
	//        return LocalRedirect(model.ReturnUrl);
	//    }
	//    if (result.IsLockedOut)
	//    {
	//        _logger.LogWarning(7, "User account locked out.");
	//        return View("Lockout");
	//    }
	//    else
	//    {
	//        ModelState.AddModelError(string.Empty, "Invalid code.");
	//        return View(model);
	//    }
	//}

	////
	//// GET: /Account/VerifyAuthenticatorCode
	//[HttpGet]
	//[AllowAnonymous]
	//public async Task<IActionResult> VerifyAuthenticatorCode(bool rememberMe, string returnUrl = null)
	//{
	//    // Require that the user has already logged in via username/password or external login
	//    var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
	//    if (user == null)
	//    {
	//        return View("Error");
	//    }
	//    return View(new VerifyAuthenticatorCodeViewModel { ReturnUrl = returnUrl, RememberMe = rememberMe });
	//}

	////
	//// POST: /Account/VerifyAuthenticatorCode
	//[HttpPost]
	//[AllowAnonymous]
	//[ValidateAntiForgeryToken]
	//public async Task<IActionResult> VerifyAuthenticatorCode(VerifyAuthenticatorCodeViewModel model)
	//{
	//    model.ReturnUrl ??= Url.Content("~/");
	//    if (!ModelState.IsValid)
	//    {
	//        return View(model);
	//    }

	//    // The following code protects for brute force attacks against the two factor codes.
	//    // If a user enters incorrect codes for a specified amount of time then the user account
	//    // will be locked out for a specified amount of time.
	//    var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(model.Code, model.RememberMe, model.RememberBrowser);
	//    if (result.Succeeded)
	//    {
	//        return LocalRedirect(model.ReturnUrl);
	//    }
	//    if (result.IsLockedOut)
	//    {
	//        _logger.LogWarning(7, "User account locked out.");
	//        return View("Lockout");
	//    }
	//    else
	//    {
	//        ModelState.AddModelError(string.Empty, "Mã sai.");
	//        return View(model);
	//    }
	//}
	////
	//// GET: /Account/UseRecoveryCode
	//[HttpGet]
	//[AllowAnonymous]
	//public async Task<IActionResult> UseRecoveryCode(string returnUrl = null)
	//{
	//    // Require that the user has already logged in via username/password or external login
	//    var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
	//    if (user == null)
	//    {
	//        return View("Error");
	//    }
	//    return View(new UseRecoveryCodeViewModel { ReturnUrl = returnUrl });
	//}

	////
	//// POST: /Account/UseRecoveryCode
	//[HttpPost]
	//[AllowAnonymous]
	//[ValidateAntiForgeryToken]
	//public async Task<IActionResult> UseRecoveryCode(UseRecoveryCodeViewModel model)
	//{
	//    model.ReturnUrl ??= Url.Content("~/");
	//    if (!ModelState.IsValid)
	//    {
	//        return View(model);
	//    }

	//    var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(model.Code);
	//    if (result.Succeeded)
	//    {
	//        return LocalRedirect(model.ReturnUrl);
	//    }
	//    else
	//    {
	//        ModelState.AddModelError(string.Empty, "Sai mã phục hồi.");
	//        return View(model);
	//    }
	//}

	[Route("access-denied")]
	[AllowAnonymous]
	public IActionResult AccessDenied()
	{
		return View();
	}
}
