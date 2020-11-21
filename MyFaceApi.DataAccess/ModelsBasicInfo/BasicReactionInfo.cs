using MyFaceApi.DataAccess.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace MyFaceApi.DataAccess.ModelsBasicInfo
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
