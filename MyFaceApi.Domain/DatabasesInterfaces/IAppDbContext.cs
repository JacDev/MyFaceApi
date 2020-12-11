using Microsoft.EntityFrameworkCore;
using MyFaceApi.Api.Domain.Entities;
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
		DbSet<Conversation> Conversations { get; set; }
		DbSet<Message> Messages { get; set; }
		Task<int> SaveAsync();
	}
}
