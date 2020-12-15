using System.ComponentModel.DataAnnotations;

namespace MyFaceApi.Api.Application.DtoModels.PostComment
{
	public class PostCommentToUpdateDto
	{
		[Required]
		public string Text { get; set; }
	}
}
