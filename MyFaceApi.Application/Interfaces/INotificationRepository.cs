using MyFaceApi.Api.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyFaceApi.Api.Repository.Interfaces
{
	public interface INotificationRepository
	{
		List<Notification> GetUserNotifications(Guid userId);
		Task<Notification> AddNotificationAsync(Notification notification);
		Task DeleteNotificationAsync(Notification notification);
		Notification GetNotification(Guid notificationId);
		//Notification GetNotification(Guid userId, Guid friendId, Guid eventId);
		Task UpdateNotificationAsync(Notification notification);
	}
}
