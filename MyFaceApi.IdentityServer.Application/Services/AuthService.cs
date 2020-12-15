using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using MyFaceApi.IdentityServer.Application.Interfaces;
using MyFaceApi.IdentityServer.Application.ViewModels;
using MyFaceApi.IdentityServer.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace MyFaceApi.IdentityServer.Application.Services
{
	public class AuthService : IAuthService
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly IIdentityServerInteractionService _interactionService;

		public AuthService(UserManager<ApplicationUser> userManager,
			SignInManager<ApplicationUser> signInManager,
			IIdentityServerInteractionService interactionService)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_interactionService = interactionService;
		}
		public async Task<SignInResult> SignUserAsync(LoginViewModel loginViewModel)
		{
			return await _signInManager.PasswordSignInAsync(loginViewModel.Login, loginViewModel.Password, false, false);
		}
		public async Task<string> LogOutUserAcync(string logoutId)
		{
			await _signInManager.SignOutAsync();

			LogoutRequest logoutRequest = await _interactionService.GetLogoutContextAsync(logoutId);

			return logoutRequest.PostLogoutRedirectUri;
		}
		public async Task RegisterUser(RegisterViewModel registerViewModel)
		{
			var user = new ApplicationUser
			{
				Id = Guid.NewGuid(),
				UserName = registerViewModel.Login,
				Email = registerViewModel.Email,
				FirstName = registerViewModel.FirstName,
				LastName = registerViewModel.LastName,
				ProfileImagePath = "",
				DateOfBirht = DateTime.Now
			};

			await _userManager.CreateAsync(user, registerViewModel.Password);
		}
	}
}
