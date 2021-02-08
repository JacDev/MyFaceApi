using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MyFaceApi.Api.Domain.DatabasesInterfaces;
using MyFaceApi.Api.Domain.Entities;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace MyFaceApi.Api.Infrastructure.Database
{
	public class OnlineUserDbContext : DbContext, IOnlineUsersDbContext
	{
		public OnlineUserDbContext(DbContextOptions<OnlineUserDbContext> options) : base(options)
		{
		}
		public DbSet<OnlineUser> OnlineUsers { get; set; }

		public override DbSet<TEntity> Set<TEntity>()
		{
			return base.Set<TEntity>();
		}
		public override EntityEntry Entry([NotNullAttribute] object entity)
		{
			return base.Entry(entity);
		}
		public async Task<int> SaveAsync()
		{
			return await SaveChangesAsync();
		}
	}
}
