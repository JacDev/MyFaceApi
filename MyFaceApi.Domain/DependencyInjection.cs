using Microsoft.Extensions.DependencyInjection;


namespace MyFaceApi.Api.Repository
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddRepositories(this IServiceCollection services)
		{
			services.AddTransient<IPostRepository, PostRepository>();
			services.AddTransient<IPostReactionRepository, PostReactionRepository>();
			services.AddTransient<IPostCommentRepository, PostCommentRepository>();
			services.AddTransient<IFriendsRelationRepository, FriendsRelationRepository>();
			services.AddTransient<IMessageRepository, MessageRepository>();
			services.AddTransient<IOnlineUsersRepository, OnlineUsersRepository>();
			services.AddTransient<INotificationRepository, NotificationRepository>();
			return services;
		}
	}
}
