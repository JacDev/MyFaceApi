using System;
using System.ComponentModel.DataAnnotations;

namespace MyFaceApi.Api.Application.DtoModels.Comment
{
	public class CommentToAddDto
	{
		[Required]
		public string Text { get; set; }
		[Required]
		public Guid FromWho { get; set; }
	}
}
