using System;

namespace MyFaceApi.Api.Application.DtoModels.PostComment
{
	public class PostCommentDto
	{
		public Guid Id { get; set; }
		public Guid PostId { get; set; }
		public string Text { get; set; }
		public Guid FromWho { get; set; }
		public DateTime WhenAdded { get; set; }
	}
}
