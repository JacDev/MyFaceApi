using MyFaceApi.Models.CommentModels;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace MyFaceApi.Entities
{
	public class PostComment : BasicCommentInfo
	{
		[Key]
		public Guid Id { get; set; }
	}
}
