using MyFaceApi.Api.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace MyFaceApi.Api.Application.DtoModels.PostReaction
{
	public class PostReactionToUpdate
	{
		[Required]
		public ReactionType Reaction { get; set; }
	}
}
