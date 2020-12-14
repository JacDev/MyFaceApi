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
			services.AddTransient<IHttpService, IdentityServerHttpService>();
			services.AddTransient<IUserService, UserService>();
			services.AddTransient<IImageManager, ImageManager>();
			services.AddTransient<IFriendsRelationService, FriendsRelationService>();
			services.AddTransient<IMessageService, MessageService>();
			services.AddTransient<INotificationService, NotificationService>();
			services.AddTransient<IOnlineUsersService, OnlineUsersService>();
			services.AddTransient<IPostService, PostService>();
			services.AddTransient<IPostReactionService, PostReactionService>();
			services.AddTransient<IPostCommentService, PostCommentService>();

			return services;
		}
	}
}
