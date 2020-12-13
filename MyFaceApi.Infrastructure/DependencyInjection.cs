using Microsoft.Extensions.DependencyInjection;
using MyFaceApi.Api.Domain.DatabasesInterfaces;
using MyFaceApi.Api.Domain.Entities;
using MyFaceApi.Api.Domain.RepositoryInterfaces;
using MyFaceApi.Api.Infrastructure.Database;
using MyFaceApi.Api.Infrastructure.Repository;

namespace MyFaceApi.Api.Infrastructure
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddInfrastructure(this IServiceCollection services)
		{
			services.AddScoped<IAppDbContext>(provider => provider.GetService<AppDbContext>());
			services.AddScoped(typeof(IRepository<Post>), typeof(Repository<Post>));
			services.AddScoped(typeof(IRepository<PostComment>), typeof(Repository<PostComment>));
			services.AddScoped(typeof(IRepository<PostReaction>), typeof(Repository<PostReaction>));
			services.AddScoped(typeof(IRepository<Notification>), typeof(Repository<Notification>));
			services.AddScoped(typeof(IRepository<Message>), typeof(Repository<Message>));
			services.AddScoped(typeof(IRepository<FriendsRelation>), typeof(Repository<FriendsRelation>));
			services.AddScoped(typeof(IRepository<OnlineUser>), typeof(Repository<OnlineUser>));

			//services.AddScoped<IRepository<Post>, Repository<Post>>();
			//services.AddScoped<IRepository<PostComment>, Repository<PostComment>>();
			//services.AddScoped<IRepository<PostReaction>, Repository<PostReaction>>();
			//services.AddScoped<IRepository<Notification>, Repository<Notification>>();
			//services.AddScoped<IRepository<Message>, Repository<Message>>();
			//services.AddScoped<IRepository<FriendsRelation>, Repository<FriendsRelation>>();
			//services.AddScoped<IRepository<OnlineUser>, Repository<OnlineUser>>();
			return services;
		}
	}
}
