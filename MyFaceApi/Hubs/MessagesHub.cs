using Microsoft.AspNetCore.SignalR;
using MyFaceApi.Api.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MyFaceApi.Api.Hubs
{
	public class MessagesHub : Hub
	{
		public override async Task OnConnectedAsync()
		{
			OnlineUserModel _loggedUser = GetLoggedUser();

			//if (!string.IsNullOrWhiteSpace(_loggedUser.Id))
			//{
			//	if (_onlineUsers.IsUserOnline(_loggedUser.Id))
			//	{
			//		await _onlineUsers.RemoveUser(_loggedUser);
			//	}
			//	else
			//	{
			//		_loggedUser.NotificationConnectionId = Context.ConnectionId;
			//		await _onlineUsers.AddOnlineUser(_loggedUser);
			//	}
			//}
			await base.OnConnectedAsync();
		}
		private OnlineUserModel GetLoggedUser()
		{
			OnlineUserModel user = new OnlineUserModel
			{
				Id = Context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value,
				FirstName = Context.User.Claims.FirstOrDefault(c => c.Type == "FirstName").Value,
				LastName = Context.User.Claims.FirstOrDefault(c => c.Type == "LastName").Value
			};
			return user;
		}
	}
}
