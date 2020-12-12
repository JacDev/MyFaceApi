using MyFaceApi.Api.Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace MyFaceApi.Api.Application.DtoModels.Notification
{
	public class NotificationToAddDto
	{
		[Required]
		public DateTime WhenAdded { get; set; }
		[Required]
		public bool HasSeen { get; set; }
		[Required]
		public Guid FromWho { get; set; }
		[Required]
		public NotificationType NotificationType { get; set; }
		[Required]
		public Guid EventId { get; set; }
	}
}
