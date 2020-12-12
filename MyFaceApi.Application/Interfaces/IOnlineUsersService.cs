using MyFaceApi.Api.Application.DtoModels.User;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyFaceApi.Api.Application.Interfaces
{
	public interface IOnlineUsersService
	{
		bool IsUserOnline(string userId);
		Task<OnlineUserDto> AddOnlineUserAsync(OnlineUserDto onlineUserModel);
		OnlineUserDto GetOnlineUser(string userId);
		Task RemoveUserAsync(string userId);
		List<string> GetOnlineUsers();
	}
}
