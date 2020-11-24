using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyFaceApi.IdentityServer.DataAccess.Entities;
using MyFaceApi.IdentityServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MyFaceApi.IdentityServer.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IIdentityServerInteractionService _interactionService;

        public AuthController(UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IIdentityServerInteractionService interactionService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _interactionService = interactionService;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(loginViewModel.Login, loginViewModel.Password, false, false);
                if (result.Succeeded)
                {
                    var x = User.Claims.FirstOrDefault(x => x.Type == "LastName");
                    return Redirect(loginViewModel.ReturnUrl);
                }
            }
            return View(new LoginViewModel { ReturnUrl = loginViewModel.ReturnUrl });
        }
        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            await _signInManager.SignOutAsync();

            var logoutRequest = await _interactionService.GetLogoutContextAsync(logoutId);

            if (string.IsNullOrEmpty(logoutRequest.PostLogoutRedirectUri))
            {
                return RedirectToAction("Index", "Home");
            }

            return Redirect(logoutRequest.PostLogoutRedirectUri);
        }

        [HttpGet]
        public IActionResult Register(string returnUrl)
        {
            return View(new RegisterViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {

            if (!ModelState.IsValid)
            {
                return View(registerViewModel);
            }
            var user = new AppUser
            {
                Id = Guid.NewGuid(),
                UserName = registerViewModel.Login,
                Email = registerViewModel.Email,
                FirstName = registerViewModel.FirstName,
                LastName = registerViewModel.LastName
            };

            var result = await _userManager.CreateAsync(user, registerViewModel.Password);

            if (result.Succeeded)
            {
                List<Claim> claims = new List<Claim> {
                    new Claim("FirstName", user.FirstName),
                    new Claim("LastName", user.LastName),
                    new Claim("ProfileImagePath", user.ProfileImagePath),
                    new Claim("DateOfBirht", user.DateOfBirht.ToString()),
                };
                await _userManager.AddClaimsAsync(user, claims);
            }         
            return RedirectToAction("Login");
        }
    }
}
