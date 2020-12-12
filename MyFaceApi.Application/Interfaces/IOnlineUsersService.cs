using MyFaceApi.Api.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyFaceApi.Api.Application.Interfaces
{
	public interface IOnlineUsersService
	{
		bool IsUserOnline(string userId);
		Task<OnlineUser> AddOnlineUserAsync(OnlineUser onlineUserModel);
		OnlineUser GetOnlineUser(string userId);
		Task RemoveUserAsync(Guid userId);
		List<Guid> GetOnlineUsers();
	}
}
