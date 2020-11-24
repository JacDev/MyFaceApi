using MyFaceApi.Api.DataAccess.ModelsBasicInfo;
using System;
using System.ComponentModel.DataAnnotations;

namespace MyFaceApi.Api.DataAccess.Entities
{
	public class PostComment : BasicCommentData
	{
		[Key]
		public Guid Id { get; set; }
	}
}
