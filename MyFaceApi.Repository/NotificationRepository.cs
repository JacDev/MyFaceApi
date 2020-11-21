using Microsoft.Extensions.Logging;
using MyFaceApi.DataAccess.Data;
using MyFaceApi.DataAccess.Entities;
using MyFaceApi.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFaceApi.Repository
{
	public class NotificationRepository : INotificationRepository
	{
		private readonly IAppDbContext _appDbContext;
		private readonly ILogger<NotificationRepository> _logger;
		public NotificationRepository(IAppDbContext appDbContext, ILogger<NotificationRepository> logger)
		{
			_appDbContext = appDbContext;
			_logger = logger;
		}
		public async Task<Notification> AddNotificationAsync(Notification notification)
		{
			_logger.LogDebug("Trying to add notification {notification}.", notification);
			if (notification is null)
			{
				throw new ArgumentNullException(nameof(Notification));
			}
			if (notification.ToWhoId == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(Guid));
			}
			try
			{
				var addedNotification = await _appDbContext.Notifications.AddAsync(notification);
				await _appDbContext.SaveAsync();
				return addedNotification.Entity;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during adding the notification.");
				throw;
			}
		}

		public async Task DeleteNotificationAsync(Notification notification)
		{
			_logger.LogDebug("Trying to remove notification: {notification}.", notification);
			if (notification is null)
			{
				throw new ArgumentNullException(nameof(Notification));
			}
			try
			{
				_appDbContext.Notifications.Remove(notification);
				await _appDbContext.SaveAsync();
				_logger.LogDebug("Notification {notification} has been removed.", notification);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during removing the notification.");
				throw;
			}
		}

		public Notification GetNotification(Guid notificationId)
		{
			_logger.LogDebug("Trying to get the notification: {notificationId}", notificationId);
			if (notificationId == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(Guid));
			}
			try
			{
				var notificationReturn = _appDbContext.Notifications.FirstOrDefault(s => s.Id == notificationId);
				return notificationReturn;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during getting the notification.");
				throw;
			}
		}

		public List<Notification> GetUserNotifications(Guid userId)
		{
			_logger.LogDebug("Trying to get user notifications: {userid}", userId);
			if (userId == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(Guid));
			}
			try
			{
				var notificationToReturn = _appDbContext.Notifications.Where(s => s.ToWhoId == userId).ToList();
				return notificationToReturn;

			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during getting the user notifications.");
				throw;
			}
		}

		public async Task UpdateNotificationAsync(Notification notification)
		{
			_logger.LogDebug("Trying to update notification: {notification}", notification);
			try
			{
				await _appDbContext.SaveAsync();
				_logger.LogDebug("Notification {notification} has been updated.", notification);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during updating the notification.");
				throw;
			}
		}
	}
}
