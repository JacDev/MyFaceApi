using MyFaceApi.Api.Domain.Enums;
using System;

namespace MyFaceApi.Api.Domain.Entities
{
	public class PostReaction
	{
		public Guid Id { get; set; }
		public Guid PostId { get; set; }
		public DateTime WhenAdded { get; set; }
		public Guid FromWho { get; set; }
		public ReactionType Reaction { get; set; }
	}
}
