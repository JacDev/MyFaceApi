using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MyFaceApi.Api.Domain.Entities;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace MyFaceApi.Api.Domain.DatabasesInterfaces
{
	public interface IOnlineUsersDbContext
	{
		EntityEntry Entry([NotNull] object entity);
		DbSet<OnlineUser> OnlineUsers { get; set; }
		DbSet<TEntity> Set<TEntity>() where TEntity : class;
		Task<int> SaveAsync();
	}
}
