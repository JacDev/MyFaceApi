using MyFaceApi.Api.DataAccess.ModelsBasicInfo;
using System;
using System.ComponentModel.DataAnnotations;

namespace MyFaceApi.Api.DataAccess.Entities
{
	public class PostReaction : BasicReactionData
	{
		[Key]
		public Guid Id { get; set; }
		[Required]
		public Guid PostId { get; set; }
	}
}
