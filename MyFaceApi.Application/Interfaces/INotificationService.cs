using Microsoft.AspNetCore.JsonPatch;
using MyFaceApi.Api.Application.DtoModels.Notification;
using Pagination.Helpers;
using System;
using System.Threading.Tasks;

namespace MyFaceApi.Api.Application.Interfaces
{
	public interface INotificationService
	{
		PagedList<NotificationDto> GetUserNotifications(Guid userId, PaginationParams paginationParams, string fromWhoId = null, int notificationType = 0);
		Task<NotificationDto> AddNotificationAsync(Guid toThoId, NotificationToAddDto notification);
		Task DeleteNotificationAsync(Guid notificationId);
		NotificationDto GetNotification(Guid notificationId);
		Task<bool> TryUpdateNotificationAsync(Guid notificationId, JsonPatchDocument<NotificationToUpdateDto> patchDocument);
	}
}
