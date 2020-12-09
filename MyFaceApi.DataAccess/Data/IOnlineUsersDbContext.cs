using Microsoft.EntityFrameworkCore;
using MyFaceApi.Api.DataAccess.Entities;
using System.Threading.Tasks;

namespace MyFaceApi.Api.DataAccess.Data
{
	public interface IOnlineUsersDbContext
	{
		DbSet<OnlineUserModel> OnlineUsers { get; set; }
		Task<int> SaveAsync();
	}
}

