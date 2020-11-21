using MyFaceApi.DataAccess.ModelsBasicInfo;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyFaceApi.DataAccess.Entities
{
	public class Post : BasicPostInfo
	{
		[Key]
		public Guid Id { get; set; }
		public DateTime WhenAdded { get; set; }
		[Required]
		public Guid UserId { get; set; }
		public virtual ICollection<PostComment> PostComments { get; set; }
		public virtual ICollection<PostReaction> PostReactions { get; set; }

		public object ElementAt(int v)
		{
			throw new NotImplementedException();
		}

		public Post()
		{
			PostComments = new List<PostComment>();
			PostReactions = new List<PostReaction>();
		}
	}
}
