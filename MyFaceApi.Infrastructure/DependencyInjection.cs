using Microsoft.Extensions.DependencyInjection;
using MyFaceApi.Api.Domain.DatabasesInterfaces;
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
			services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

			return services;
		}
	}
}
