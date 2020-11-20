using MyFaceApi.Models.PostReactionModels;
using System;
using System.ComponentModel.DataAnnotations;

namespace MyFaceApi.Entities
{
	public class PostReaction : BasicReactionInfo
	{
		[Key]
		public Guid Id { get; set; }
		[Required]
		public Guid PostId { get; set; }
	}
}
