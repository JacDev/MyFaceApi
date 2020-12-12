using System;
using System.Collections.Generic;

namespace MyFaceApi.Api.Domain.Entities
{
	public class Post
	{
		public Guid Id { get; set; }
		public DateTime WhenAdded { get; set; }
		public Guid UserId { get; set; }
		public string Text { get; set; }
		public string ImagePath { get; set; }
		public string ImageFullPath { get; set; }
		public virtual ICollection<PostComment> PostComments { get; set; }
		public virtual ICollection<PostReaction> PostReactions { get; set; }
	}
}
