using Microsoft.Extensions.DependencyInjection;
using MyFaceApi.Api.Domain.Entities;
using MyFaceApi.Api.Domain.RepositoryInterfaces;
using MyFaceApi.Api.Infrastructure.Repository;

namespace MyFaceApi.Api.Infrastructure
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddInfrastructure(this IServiceCollection services)
		{
			services.AddScoped<IRepository<Post>, Repository<Post>>();
			services.AddScoped<IRepository<PostComment>, Repository<PostComment>>();
			services.AddScoped<IRepository<PostReaction>, Repository<PostReaction>>();
			services.AddScoped<IRepository<Notification>, Repository<Notification>>();
			services.AddScoped<IRepository<Message>, Repository<Message>>();
			services.AddScoped<IRepository<FriendsRelation>, Repository<FriendsRelation>>();
			services.AddScoped<IRepository<OnlineUser>, Repository<OnlineUser>>();
			return services;
		}
	}
}
