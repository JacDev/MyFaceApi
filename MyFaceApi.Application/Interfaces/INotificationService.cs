using Microsoft.AspNetCore.JsonPatch;
using MyFaceApi.Api.Application.DtoModels.Notification;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyFaceApi.Api.Application.Interfaces
{
	public interface INotificationService
	{
		List<NotificationDto> GetUserNotifications(Guid userId);
		Task<NotificationDto> AddNotificationAsync(Guid toThoId, NotificationToAddDto notification);
		Task DeleteNotificationAsync(Guid notificationId);
		NotificationDto GetNotification(Guid notificationId);
		Task<bool> TryUpdateNotificationAsync(Guid notificationId, JsonPatchDocument<NotificationToUpdateDto> patchDocument);    }
}
