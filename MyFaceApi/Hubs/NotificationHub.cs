using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using MyFaceApi.Api.Application.DtoModels.Message;
using MyFaceApi.Api.Application.DtoModels.Notification;
using MyFaceApi.Api.Application.DtoModels.User;
using MyFaceApi.Api.Application.Interfaces;
using MyFaceApi.Api.Domain.Enums;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MyFaceApi.Api.Hubs
{
	public class NotificationHub : Hub
	{
		private readonly IOnlineUsersService _onlineUsersService;
		private readonly IMessageService _messageService;
		private readonly ILogger<NotificationHub> _logger;
		private readonly INotificationService _notificationService;

		public NotificationHub(IOnlineUsersService onlineUsersService,
			IMessageService messageService,
			ILogger<NotificationHub> logger,
			INotificationService notificationService)
		{
			_onlineUsersService = onlineUsersService;
			_messageService = messageService;
			_logger = logger;
			_notificationService = notificationService;
		}
		public async Task SendMessageToUser(string toWhoId, string message, DateTime when)
		{
			try
			{
				var fromWhoUser = GetLoggedUser().Id;
				if (!string.IsNullOrWhiteSpace(toWhoId) && Guid.TryParse(toWhoId, out Guid gToWhoId))
				{
					if (_onlineUsersService.IsUserOnline(toWhoId))
					{
						string toWhoConnectionId = _onlineUsersService.GetOnlineUser(toWhoId).ConnectionId;
						await Clients.Client(toWhoConnectionId).SendAsync("ReceiveMessage", fromWhoUser, message, when);
					}
					await _messageService.AddMessageAsync(gToWhoId, new MessageToAddDto
					{
						FromWho = Guid.Parse(fromWhoUser),
						Text = message,
						When = when
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
		public async Task SendNotificationToUser(string toWhoId, string type, string when, string eventId)
		{
			try
			{
				var notificationType = type switch
				{
					"comment" => NotificationType.Comment,
					"friendRequiest" => NotificationType.FriendRequiest,
					"reaction" => NotificationType.Reaction,
					"friendRequiestAccepted" => NotificationType.FriendRequiestAccepted,
					_ => throw new ArgumentNullException(nameof(NotificationType)),
				};

				var fromWhoUser = GetLoggedUser().Id;
				if (!string.IsNullOrWhiteSpace(toWhoId) && Guid.TryParse(toWhoId, out Guid gToWhoId) && Guid.TryParse(eventId, out Guid gEventId))
				{
					if (_onlineUsersService.IsUserOnline(toWhoId))
					{
						string toWhoConnectionId = _onlineUsersService.GetOnlineUser(toWhoId).ConnectionId;
						await Clients.Client(toWhoConnectionId).SendAsync("ReceiveNotification");
					}
					await _notificationService.AddNotificationAsync(gToWhoId, new NotificationToAddDto
					{
						FromWho = Guid.Parse(fromWhoUser),
						WhenAdded = Convert.ToDateTime(when),
						EventId = gEventId,
						HasSeen = false,
						NotificationType = notificationType
					});
				}
				else
				{
					throw new ArgumentNullException(nameof(toWhoId));
				}
			}
			catch (Exception ex)
			{
				_logger.LogError("Something went wrong during sending notification to the user: {userId}", toWhoId);
				_logger.LogError("{0}", ex);
			}
			return;
		}
		public override async Task OnConnectedAsync()
		{
			OnlineUserDto _loggedUser = GetLoggedUser();
			if (!string.IsNullOrWhiteSpace(_loggedUser.Id))
			{
				if (_onlineUsersService.IsUserOnline(_loggedUser.Id))
				{
					_loggedUser.ConnectionId = Context.ConnectionId;
				}
				else
				{
					_loggedUser.ConnectionId = Context.ConnectionId;
					await _onlineUsersService.AddOnlineUserAsync(_loggedUser);
				}
			}
			await base.OnConnectedAsync();
		}
		public override async Task OnDisconnectedAsync(Exception ex)
		{
			OnlineUserDto _loggedUser = GetLoggedUser();
			if (_loggedUser != null)
			{
				await _onlineUsersService.RemoveUserAsync(_loggedUser.Id);
			}
			await base.OnDisconnectedAsync(ex);
		}
		private OnlineUserDto GetLoggedUser()
		{
			OnlineUserDto user = new OnlineUserDto();
			if (Context.User.Claims != null)
			{
				user.Id = Context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
			}
			return user;
		}
	}
}
