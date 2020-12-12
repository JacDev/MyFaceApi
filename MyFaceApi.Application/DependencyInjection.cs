using Microsoft.Extensions.DependencyInjection;
using MyFaceApi.Api.Application.Interfaces;
using MyFaceApi.Api.Application.Services;
using System.Reflection;
using AutoMapper;

namespace MyFaceApi.Api.Application
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddRepositories(this IServiceCollection services)
		{
			services.AddAutoMapper(Assembly.GetExecutingAssembly());
			services.AddScoped<IFriendsRelationService, FriendsRelationService>();
			services.AddScoped<IMessageService, MessageService>();
			services.AddScoped<INotificationService, NotificationService>();
			services.AddScoped<IOnlineUsersService, OnlineUsersService>();
			services.AddScoped<IPostService, PostService>();
			services.AddScoped<IPostReactionService, PostReactionService>();
			services.AddScoped<IPostCommentService, PostCommentService>();
			return services;
		}
	}
}
