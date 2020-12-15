using MyFaceApi.Api.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace MyFaceApi.Api.Application.DtoModels.PostReaction
{
	public class PostReactionToUpdateDto
	{
		[Required]
		public ReactionType Reaction { get; set; }
	}
}
