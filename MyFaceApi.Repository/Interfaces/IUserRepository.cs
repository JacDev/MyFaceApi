using MyFaceApi.Api.DataAccess.Entities;
using System;
using System.Threading.Tasks;

namespace MyFaceApi.Api.Repository.Interfaces
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
