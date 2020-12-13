using System;

namespace MyFaceApi.Api.Application.DtoModels.Comment
{
	public class CommentDto
	{
		public Guid Id { get; set; }
		public Guid PostId { get; set; }
		public string Text { get; set; }
		public Guid FromWho { get; set; }
		public DateTime WhenAdded { get; set; }
	}
}
