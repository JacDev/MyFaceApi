using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using MyFaceApi.IdentityServer.Application.Interfaces;
using MyFaceApi.IdentityServer.Application.Services;
using System.Reflection;


namespace MyFaceApi.IdentityServer.Application
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddApplication(this IServiceCollection services)
		{
			services.AddAutoMapper(Assembly.GetExecutingAssembly());
			services.AddTransient<IIdentityUserService, IdentityUserService>();
			services.AddTransient<IAuthService, AuthService>();
			return services;
		}
	}
}
