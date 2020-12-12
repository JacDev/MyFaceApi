using System.ComponentModel.DataAnnotations;

namespace MyFaceApi.Api.Application.DtoModels.Comment
{
	public class CommentToUpdateDto
	{
		[Required]
		public string Text { get; set; }
	}
}
