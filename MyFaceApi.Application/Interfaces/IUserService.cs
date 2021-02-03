using MyFaceApi.Api.Application.DtoModels.User;
using Pagination.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyFaceApi.Api.Application.Interfaces
{
	public interface IUserService
	{
		Task<UserDto> GetUserAsync(Guid userId);
		Task<bool> CheckIfUserExists(Guid userId);
		Task<List<UserDto>> GetUsersAsync(IEnumerable<Guid> usersId);
		Task<PagedList<UserDto>> GetUsersAsync(string searchString, PaginationParams paginationParams);
		Task<UserDto> UpdateProfilePath(Guid userId, string path);
	}
}