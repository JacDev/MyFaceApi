using MyFaceApi.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyFaceApi.Entities
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
