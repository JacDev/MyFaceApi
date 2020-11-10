using MyFaceApi.Entities;
using System;
using System.Threading.Tasks;

namespace MyFaceApi.Repository
{
	public interface IUserRepository
	{
		Task<User> AddUserAcync(User user);
		Task DeleteUserAsync(User user);
		Task<User> GetUserAsync(Guid userId);
		Task UpdateUserAsync(User user);
	}
}
