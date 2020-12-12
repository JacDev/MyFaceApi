using Microsoft.Extensions.DependencyInjection;
using MyFaceApi.Api.Application.Interfaces;
using MyFaceApi.Api.Application.Services;
using System.Reflection;
using AutoMapper;
using MyFaceApi.Api.Application.IdentityServerAccess;
using MyFaceApi.Api.Domain.ExternalApiInterfaces;
using MyFaceApi.Api.Application.FileManager;
using MyFaceApi.Api.Application.FileManagerInterfaces;

namespace MyFaceApi.Api.Application
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddApplication(this IServiceCollection services)
		{
			services.AddAutoMapper(Assembly.GetExecutingAssembly());
			services.AddScoped<IUserService, UserService>();
			services.AddScoped<IImageManager, ImageManager>();
			services.AddScoped<IFriendsRelationService, FriendsRelationService>();
			services.AddScoped<IMessageService, MessageService>();
			services.AddScoped<INotificationService, NotificationService>();
			services.AddScoped<IOnlineUsersService, OnlineUsersService>();
			services.AddScoped<IPostService, PostService>();
			services.AddScoped<IPostReactionService, PostReactionService>();
			services.AddScoped<IPostCommentService, PostCommentService>();
			services.AddScoped<IHttpService, IdentityServerHttpService>();

			return services;
		}
	}
}
