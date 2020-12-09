using Microsoft.EntityFrameworkCore;
using MyFaceApi.Api.DataAccess.Entities;
using System.Threading.Tasks;

namespace MyFaceApi.Api.DataAccess.Data
{
	public class OnlineUsersDbContext : DbContext, IOnlineUsersDbContext
	{
		public OnlineUsersDbContext(DbContextOptions<OnlineUsersDbContext> options) : base(options)
		{
		}
		public DbSet<OnlineUserModel> OnlineUsers { get; set; }

		public async Task<int> SaveAsync()
		{
			return await SaveChangesAsync();
		}
	}
}
