using MyFaceApi.Api.Application.DtoModels.User;
using MyFaceApi.Api.Application.Helpers;
using System;
using System.Threading.Tasks;

namespace MyFaceApi.Api.Application.Interfaces
{
	public interface IOnlineUsersService
	{
		bool IsUserOnline(string userId);
		Task<OnlineUserDto> AddOnlineUserAsync(OnlineUserDto onlineUserModel);
		OnlineUserDto GetOnlineUser(string userId);
		Task RemoveUserAsync(string userId);
		Task<PagedList<UserDto>> GetOnlineFriends(Guid userId, PaginationParams paginationParams);
	}
}
