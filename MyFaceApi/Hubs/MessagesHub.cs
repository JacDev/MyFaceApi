
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using MyFaceApi.Api.DataAccess.Entities;
using MyFaceApi.Api.Repository.Interfaces;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MyFaceApi.Api.Hubs
{
	public class MessagesHub : Hub
	{
		private readonly IOnlineUsersRepository _onlineUsersRepository;
		private readonly IMessageRepository _messageRepository;
		private readonly ILogger<MessagesHub> _logger;
		private readonly INotificationRepository _notificationRepository;

		public MessagesHub(IOnlineUsersRepository onlineUsersRepository,
			IMessageRepository messageRepository,
			ILogger<MessagesHub> logger,
			INotificationRepository notificationRepository)
		{
			_onlineUsersRepository = onlineUsersRepository;
			_messageRepository = messageRepository;
			_logger = logger;
			_notificationRepository = notificationRepository;
		}
		public async Task SendMessageToUser(string toWhoId, string message)
		{
			try
			{
				var fromWhoUser = GetLoggedUser().Id;
				if (!string.IsNullOrWhiteSpace(toWhoId) && Guid.TryParse(fromWhoUser, out Guid gToWhoId))
				{
					if (_onlineUsersRepository.IsUserOnline(toWhoId))
					{
						string toWhoConnectionId = _onlineUsersRepository.GetOnlineUser(toWhoId).ConnectionId;
						await Clients.Client(toWhoConnectionId).SendAsync("ReceiveMessage", fromWhoUser, message);
					}
					await _messageRepository.AddMessageAsync(new Message
					{
						FromWho = Guid.Parse(fromWhoUser),
						Text = message,
						ToWho = gToWhoId,
						When = DateTime.Now
					});
				}
				else
				{
					throw new ArgumentNullException(nameof(toWhoId));
				}
			}
			catch (Exception ex)
			{
				_logger.LogError("Something went wrong during sending message to the user: {userId}", toWhoId);
				_logger.LogError("{0}", ex);
			}
			return;
		}
		public override async Task OnConnectedAsync()
		{
			OnlineUserModel _loggedUser = GetLoggedUser();
			if (!string.IsNullOrWhiteSpace(_loggedUser.Id))
			{
				if (_onlineUsersRepository.IsUserOnline(_loggedUser.Id))
				{
					await _onlineUsersRepository.RemoveUser(_loggedUser);
				}
				else
				{
					_loggedUser.ConnectionId = Context.ConnectionId;
					await _onlineUsersRepository.AddOnlineUser(_loggedUser);
				}
			}
			await base.OnConnectedAsync();
		}
		public override async Task OnDisconnectedAsync(Exception ex)
		{
			OnlineUserModel _loggedUser = GetLoggedUser();
			if (_loggedUser != null)
			{
				await _onlineUsersRepository.RemoveUser(_loggedUser);
			}
			await base.OnDisconnectedAsync(ex);
		}
		private OnlineUserModel GetLoggedUser()
		{
			try
			{
				if (Context.User.Claims != null)
				{
					OnlineUserModel user = new OnlineUserModel
					{
						Id = Context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value,
						//FirstName = Context.User.Claims.FirstOrDefault(c => c.Type == "FirstName").Value,
						//LastName = Context.User.Claims.FirstOrDefault(c => c.Type == "LastName").Value
					};
					return user;
				}
			}
			catch
			{

			}
			return null;
		}
	}
}
