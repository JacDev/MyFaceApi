using MyFaceApi.Api.Domain.Enums;
using System;

namespace MyFaceApi.Api.Application.DtoModels.Notification
{
	public class NotificationDto
	{
		public Guid Id { get; set; }
		public DateTime WhenAdded { get; set; }
		public bool HasSeen { get; set; }
		public Guid FromWho { get; set; }
		public NotificationType NotificationType { get; set; }
		public Guid EventId { get; set; }
	}
}
