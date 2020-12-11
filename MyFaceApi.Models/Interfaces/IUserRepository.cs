using MyFaceApi.Api.DataAccess.Entities;
using MyFaceApi.Api.DataAccess.ModelsBasicInfo;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyFaceApi.Api.Repository.Interfaces
{
	public interface IUserRepository
	{
		Task<User> AddUserAcync(User user);
		Task DeleteUserAsync(User user);
		Task<BasicUserData> GetUserAsync(Guid userId);
		Task<List<BasicUserData>> GetUsersAsync(IEnumerable<Guid> usersId);
		Task UpdateUserAsync(User user);
		Task<bool> CheckIfUserExists(Guid userId);
	}
}
