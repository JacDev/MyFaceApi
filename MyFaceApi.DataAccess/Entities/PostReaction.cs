using MyFaceApi.DataAccess.ModelsBasicInfo;
using System;
using System.ComponentModel.DataAnnotations;

namespace MyFaceApi.DataAccess.Entities
{
	public class PostReaction : BasicReactionInfo
	{
		[Key]
		public Guid Id { get; set; }
		[Required]
		public Guid PostId { get; set; }
	}
}
