using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MyFaceApi.Api.Domain.DatabasesInterfaces;
using MyFaceApi.Api.Domain.Entities;
using System.Diagnostics.CodeAnalysis;
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
		public DbSet<OnlineUser> OnlineUsers { get; set; }
		public DbSet<Message> Messages { get; set; }
		public override DbSet<TEntity> Set<TEntity>()
		{
			return base.Set<TEntity>();
		}
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.Entity<FriendsRelation>().HasKey(s => new { s.UserId, s.FriendId });
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
