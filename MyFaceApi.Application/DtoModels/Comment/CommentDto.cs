using System;

namespace MyFaceApi.Api.Application.DtoModels.Comment
{
	public class CommentDto
	{
		public string Text { get; set; }
		public Guid FromWho { get; set; }
		public DateTime WhenAdded { get; set; }
	}
}
