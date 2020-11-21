using System.ComponentModel.DataAnnotations;

namespace MyFaceApi.Models.CommentModels
{
	public class CommentToUpdate
	{
		[Required]
		public string Text { get; set; }
	}
}
