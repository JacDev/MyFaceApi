using MyFaceApi.Api.Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace MyFaceApi.Api.Application.DtoModels.PostReaction
{
	public class PostReactionToAddDto
	{
		[Required]
		public Guid FromWho { get; set; }
		[Required]
		public ReactionType Reaction { get; set; }
	}
}
