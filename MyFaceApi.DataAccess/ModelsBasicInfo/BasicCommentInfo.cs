using System;
using System.ComponentModel.DataAnnotations;

namespace MyFaceApi.DataAccess.ModelsBasicInfo
{
	public class BasicCommentInfo
	{
		[Required]
		public string Text { get; set; }
		[Required]
		public Guid FromWho { get; set; }
		[Required]
		public Guid PostId { get; set; }
		[Required]
		public DateTime WhenAdded { get; set; }
	}
}
