using MyFaceApi.DataAccess.ModelsBasicInfo;
using System;
using System.ComponentModel.DataAnnotations;

namespace MyFaceApi.DataAccess.Entities
{
	public class PostComment : BasicCommentInfo
	{
		[Key]
		public Guid Id { get; set; }
	}
}
