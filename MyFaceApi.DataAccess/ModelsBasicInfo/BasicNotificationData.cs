using MyFaceApi.Api.DataAccess.Enums;
using System;

namespace MyFaceApi.Api.DataAccess.ModelsBasicInfo
{
	public class BasicNotificationData
	{
		public DateTime WhenAdded { get; set; }
		public bool HasSeen { get; set; }
		public Guid FromWho { get; set; }
		public NotificationType NotificationType { get; set; }
		public Guid EventId { get; set; }
	}
}
