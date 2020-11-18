using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyFaceApi.Entities;
using System;
using System.Threading.Tasks;

namespace MyFaceApi.Data
{
	public class AppDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>, IAppDbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
		{
		}
		public DbSet<FriendRelation> Relations { get; set; }
		public DbSet<Post> Posts { get; set; }
		public DbSet<Notification> Notifications { get; set; }
		public DbSet<PostComment> PostComments { get; set; }
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.Entity<FriendRelation>().HasKey(s => new { s.UserId, s.FriendId });
		}
		public async Task<int> SaveAsync()
		{
			return await SaveChangesAsync();
		}
	}
}
