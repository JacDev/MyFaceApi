using Microsoft.EntityFrameworkCore;
using MyFaceApi.Api.DataAccess.Data;
using MyFaceApi.Api.DataAccess.Entities;
using System.Threading.Tasks;

namespace MyFaceApi.Api.Infrastructure.Database
{
	public class AppDbContext : DbContext, IAppDbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
		{
		}
		public DbSet<FriendsRelation> Relations { get; set; }
		public DbSet<Post> Posts { get; set; }
		public DbSet<Notification> Notifications { get; set; }
		public DbSet<PostComment> PostComments { get; set; }
		public DbSet<PostReaction> PostReactions { get; set; }
		public DbSet<Conversation> Conversations { get; set; }
		public DbSet<Message> Messages { get; set; }
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.Entity<FriendsRelation>().HasKey(s => new { s.UserId, s.FriendId });
		}
		public async Task<int> SaveAsync()
		{
			return await SaveChangesAsync();
		}
	}
}
