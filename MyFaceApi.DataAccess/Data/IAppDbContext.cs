using Microsoft.EntityFrameworkCore;
using MyFaceApi.Api.DataAccess.Entities;
using System.Threading.Tasks;

namespace MyFaceApi.Api.DataAccess.Data
{
	public interface IAppDbContext
	{
		DbSet<User> Users { get; set; }
		DbSet<FriendsRelation> Relations { get; set; }
		DbSet<Post> Posts { get; set; }
		DbSet<Notification> Notifications { get; set; }
		DbSet<PostComment> PostComments { get; set; }
		DbSet<PostReaction> PostReactions { get; set; }
		DbSet<Conversation> Conversations { get; set; }
		DbSet<Message> Messages { get; set; }
		Task<int> SaveAsync();
	}
}
