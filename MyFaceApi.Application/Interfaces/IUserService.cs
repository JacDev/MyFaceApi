using MyFaceApi.Api.Application.DtoModels.User;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyFaceApi.Api.Application.Interfaces
{
	public interface IUserService
	{
		Task<UserWithCountersDbo> GetUserAsync(Guid userId);
		Task<bool> GetUserIfExists(Guid userId);
		Task<List<UserWithCountersDbo>> GetUsersAsync(IEnumerable<Guid> usersId);
	}
}
