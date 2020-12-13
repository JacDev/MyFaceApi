using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MyFaceApi.Api.Domain.Entities;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace MyFaceApi.Api.Domain.DatabasesInterfaces
{
	public interface IAppDbContext
	{
		DbSet<FriendsRelation> Relations { get; set; }
		DbSet<Post> Posts { get; set; }
		DbSet<Notification> Notifications { get; set; }
		DbSet<PostComment> PostComments { get; set; }
		DbSet<PostReaction> PostReactions { get; set; }
		DbSet<OnlineUser> OnlineUsers { get; set; }
		DbSet<Message> Messages { get; set; }
		DbSet<TEntity> Set<TEntity>() where TEntity : class;
		EntityEntry Entry([NotNull] object entity);
		Task<int> SaveAsync();
	}
}
