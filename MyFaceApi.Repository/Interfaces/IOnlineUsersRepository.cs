using MyFaceApi.Api.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyFaceApi.Api.Repository.Interfaces
{
	public interface IOnlineUsersRepository
	{
		bool IsUserOnline(string userId);
		Task AddOnlineUser(OnlineUserModel onlineUserModel);
		OnlineUserModel GetOnlineUser(string userId);
		Task RemoveUser(OnlineUserModel user);
		Task<List<Guid>> GetOnlineUsersAsync();
	}
}
