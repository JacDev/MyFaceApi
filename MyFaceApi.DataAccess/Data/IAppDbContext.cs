using Microsoft.EntityFrameworkCore;
using MyFaceApi.DataAccess.Entities;
using System.Threading.Tasks;

namespace MyFaceApi.DataAccess.Data
{
	public interface IAppDbContext
	{
		DbSet<User> Users { get; set; }
		DbSet<FriendRelation> Relations { get; set; }
		DbSet<Post> Posts { get; set; }
		DbSet<Notification> Notifications { get; set; }
		DbSet<PostComment> PostComments { get; set; }
		DbSet<PostReaction> PostReactions { get; set; }
		Task<int> SaveAsync();
	}
}
