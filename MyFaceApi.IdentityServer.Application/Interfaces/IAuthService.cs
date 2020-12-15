using Microsoft.AspNetCore.Identity;
using MyFaceApi.IdentityServer.Application.ViewModels;
using System.Threading.Tasks;

namespace MyFaceApi.IdentityServer.Application.Interfaces
{
	public interface IAuthService
	{
		Task<string> LogOutUserAcync(string logoutId);
		Task RegisterUser(RegisterViewModel registerViewModel);
		Task<SignInResult> SignUserAsync(LoginViewModel loginViewModel);
	}
}