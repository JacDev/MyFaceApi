using System;
using System.ComponentModel.DataAnnotations;

namespace MyFaceApi.Api.DataAccess.ModelsBasicInfo
{
	public class BasicCommentData
	{
		[Required]
		public string Text { get; set; }
		public Guid FromWho { get; set; }
	}
}
