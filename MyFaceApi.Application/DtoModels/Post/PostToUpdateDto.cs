using System.ComponentModel.DataAnnotations;

namespace MyFaceApi.Api.Application.DtoModels.Post
{
	public class PostToUpdateDto
	{
		[Required]
		public string Text { get; set; }
	}
}
