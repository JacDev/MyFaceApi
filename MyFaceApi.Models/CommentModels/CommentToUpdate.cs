using System.ComponentModel.DataAnnotations;

namespace MyFaceApi.Api.Models.CommentModels
{
	public class CommentToUpdate
	{
		[Required]
		public string Text { get; set; }
	}
}
