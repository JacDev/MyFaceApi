using MyFaceApi.DataAccess.Enums;
using System;

namespace MyFaceApi.DataAccess.ModelsBasicInfo
{
	public class BasicNotificationInfo
	{
		public DateTime WhenAdded { get; set; }
		public bool HasSeen { get; set; }
		public Guid FromWho { get; set; }
		public NotificationType NotificationType { get; set; }
		public Guid EventId { get; set; }
	}
}
