using MyFaceApi.Api.DataAccess.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace MyFaceApi.Api.DataAccess.ModelsBasicInfo
{
	public class BasicReactionData
	{
		[Required]
		public Guid FromWho { get; set; }
		[Required]
		public DateTime WhenAdded { get; set; }
		public ReactionType Reaction { get; set; }
	}
}
