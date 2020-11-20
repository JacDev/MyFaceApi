using MyFaceApi.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace MyFaceApi.Models.PostReactionModels
{
	public class BasicReactionInfo
	{
		[Required]
		public Guid FromWho { get; set; }
		[Required]
		public DateTime WhenAdded { get; set; }
		public ReactionType Reaction { get; set; }
	}
}
