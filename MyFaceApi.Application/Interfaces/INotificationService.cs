using MyFaceApi.Api.Application.DtoModels.Notification;
using MyFaceApi.Api.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyFaceApi.Api.Application.Interfaces
{
	public interface INotificationService
	{
		List<NotificationDto> GetUserNotifications(Guid userId);
		Task<NotificationDto> AddNotificationAsync(Guid notificationId, NotificationToAddDto notification);
		Task DeleteNotificationAsync(Guid notificationId);
		NotificationDto GetNotification(Guid notificationId);
		Task UpdateNotificationAsync(Notification notification);
	}
}
