using MyFaceApi.DataAccess.ModelsBasicInfo;
using System;
using System.ComponentModel.DataAnnotations;

namespace MyFaceApi.DataAccess.Entities
{
	public class Notification : BasicNotificationData
	{
		[Key]
		public Guid Id { get; set; }
		public Guid ToWhoId { get; set; }
	}
}
