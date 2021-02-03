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
        public IActionResult Login(string returnUrl, bool seenNotification = false)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl, AlreadySeenNotificaton = seenNotification });
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
            return View(new LoginViewModel { ReturnUrl = loginViewModel.ReturnUrl, AlreadySeenNotificaton = true });
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
            return View(new RegisterViewModel { ReturnUrl = returnUrl , AlreadySeenNotification = seenNotification });
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(registerViewModel);
            }
            await _authService.RegisterUser(registerViewModel);

            return RedirectToAction("Login", new {
                returnUrl = registerViewModel.ReturnUrl});
        }
    }
}
