using System;

namespace MyFaceApi.Api.Domain.Entities
{
	public class PostComment
	{
		public Guid Id { get; set; }
		public Guid PostId { get; set; }
		public Guid FromWho { get; set; }
		public DateTime WhenAdded { get; set; }
		public string Text { get; set; }
	}
}
