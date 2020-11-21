using MyFaceApi.DataAccess.Entities;
using System;
using System.Threading.Tasks;

namespace MyFaceApi.Repository.Interfaces
{
	public interface IUserRepository
	{
		Task<User> AddUserAcync(User user);
		Task DeleteUserAsync(User user);
		Task<User> GetUserAsync(Guid userId);
		Task UpdateUserAsync(User user);
		bool CheckIfUserExists(Guid userId);
	}
}
