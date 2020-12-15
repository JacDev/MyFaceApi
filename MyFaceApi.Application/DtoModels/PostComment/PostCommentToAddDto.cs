using System;
using System.ComponentModel.DataAnnotations;

namespace MyFaceApi.Api.Application.DtoModels.PostComment
{
	public class PostCommentToAddDto
	{
		[Required]
		public string Text { get; set; }
		[Required]
		public Guid FromWho { get; set; }
		[Required]
		public DateTime WhenAdded { get; set; }
	}
}
