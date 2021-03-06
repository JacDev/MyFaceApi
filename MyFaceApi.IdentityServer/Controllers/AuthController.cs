﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyFaceApi.IdentityServer.Application.Interfaces;
using MyFaceApi.IdentityServer.Application.ViewModels;
using System.Threading.Tasks;

namespace MyFaceApi.IdentityServer.Controllers
{
	public class AuthController : Controller
	{
		private readonly IAuthService _authService;


		public AuthController(IAuthService authService)
		{
			_authService = authService;
		}
		[HttpGet]
		public IActionResult Login(string returnUrl, bool seenNotification = false, bool successfulRegistration = false)
		{
			return View(new LoginViewModel
			{
				ReturnUrl = returnUrl,
				AlreadySeenNotificaton = seenNotification,
				SuccessfulRegistration = successfulRegistration
			});
		}
		[HttpPost]
		public async Task<IActionResult> Login(LoginViewModel loginViewModel)
		{
			if (ModelState.IsValid)
			{
				var result = await _authService.SignUserAsync(loginViewModel);
				if (result.Succeeded)
				{
					return Redirect(loginViewModel.ReturnUrl);
				}
			}
			return View(new LoginViewModel { ReturnUrl = loginViewModel.ReturnUrl, AlreadySeenNotificaton = true, ErrorMessage = "Podany login lub hasło są nieprawiidłowe!" });
		}
		[HttpGet]
		public async Task<IActionResult> Logout(string logoutId)
		{
			string logoutRedirect = await _authService.LogOutUserAcync(logoutId);

			if (string.IsNullOrEmpty(logoutRedirect))
			{
				return RedirectToAction("Index", "Home");
			}

			return Redirect(logoutRedirect);
		}

		[HttpGet]
		public IActionResult Register(string returnUrl, bool seenNotification = false)
		{
			return View(new RegisterViewModel { ReturnUrl = returnUrl, AlreadySeenNotification = seenNotification });
		}
		[HttpPost]
		public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
		{
			if (!ModelState.IsValid)
			{
				return View(registerViewModel);
			}
			IdentityResult result = await _authService.RegisterUser(registerViewModel);
			if (result.Succeeded)
			{
				return RedirectToAction("Login", new
				{
					returnUrl = registerViewModel.ReturnUrl,
					seenNotification = true,
					successfulRegistration = true
				});
			}
			else
			{
				foreach (var err in result.Errors)
				{
					if (err.Code == "DuplicateUserName")
					{
						registerViewModel.ErrorMessage = "Podany login jest zajęty";
					}
					else if (err.Code == "InvalidUserName")
					{
						registerViewModel.ErrorMessage = "Login może skłądać się tylko z liter i cyfr, bez polskich znaków";
					}
				}
				//if(DuplicateUserName)

				return View(registerViewModel);
			}

		}
	}
}
