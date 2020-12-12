using System;

namespace MyFaceApi.Api.Application.DtoModels.Post
{
	public class PostDto
	{
		public Guid Id { get; set; }
		public DateTime WhenAdded { get; set; }
		public Guid UserId { get; set; }
		public string Text { get; set; }
		public string ImagePath { get; set; }
		public int PostCommentsCounter { get; set; }
		public int PostReactionsCounter { get; set; }
	}
}
