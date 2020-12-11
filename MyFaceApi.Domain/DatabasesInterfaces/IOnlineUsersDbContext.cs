using Microsoft.EntityFrameworkCore;
using MyFaceApi.Api.Domain.Entities;
using System.Threading.Tasks;

namespace MyFaceApi.Api.Domain.DatabasesInterfaces
{
	public interface IOnlineUsersDbContext
	{
		DbSet<OnlineUserModel> OnlineUsers { get; set; }
		Task<int> SaveAsync();
	}
}

