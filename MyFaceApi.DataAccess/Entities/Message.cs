using MyFaceApi.DataAccess.ModelsBasicInfo;
using System;
using System.ComponentModel.DataAnnotations;

namespace MyFaceApi.DataAccess.Entities
{
	public class Message : BasicMessageData
	{
		[Key]
		public Guid Id { get; set; }
		public Guid ToWho { get; set; }
		public Guid ConversationId { get; set; }
	}
}
