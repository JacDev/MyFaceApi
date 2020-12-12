using MyFaceApi.Api.Domain.Enums;
using System;

namespace MyFaceApi.Api.Application.DtoModels.PostReaction
{
	public class PostReactionDto
	{
		public Guid PostId { get; set; }
		public DateTime WhenAdded { get; set; }
		public Guid FromWho { get; set; }
		public ReactionType Reaction { get; set; }
	}
}
