using MyFaceApi.DataAccess.ModelsBasicInfo;
using System;
using System.ComponentModel.DataAnnotations;

namespace MyFaceApi.DataAccess.Entities
{
	public class Notification : BasicNotificationInfo
	{
		[Key]
		public Guid Id { get; set; }
		public Guid ToWhoId { get; set; }
	}
}
