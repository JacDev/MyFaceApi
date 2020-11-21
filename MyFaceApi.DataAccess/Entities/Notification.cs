using MyFaceApi.DataAccess.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace MyFaceApi.DataAccess.Entities
{
	public class Notification
	{
		[Key]
		public Guid Id { get; set; }
		public bool HasSeen { get; set; }
		public Guid FromWho { get; set; }
		public Guid ToWhoId { get; set; }
		public NotificationType NotificationType { get; set; }
		public Guid EventId { get; set; }
	}
}
