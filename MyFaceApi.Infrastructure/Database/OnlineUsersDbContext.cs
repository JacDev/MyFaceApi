using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace MyFaceApi.Api.Infrastructure.Database
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
