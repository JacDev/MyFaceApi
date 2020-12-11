using MyFaceApi.Api.Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace MyFaceApi.Api.Domain.Entities
{
	public class PostReaction
	{
		public Guid Id { get; set; }
		[Required]
		public Guid PostId { get; set; }
		public DateTime WhenAdded { get; set; }
		public Guid FromWho { get; set; }
		public ReactionType Reaction { get; set; }
	}
}
