using MyFaceApi.Models.PostModels;
using System;
using System.ComponentModel.DataAnnotations;

namespace MyFaceApi.Entities
{
	public class Post : BasicPostInfo
	{
		[Key]
		public Guid Id { get; set; }
		public DateTime WhenAdded { get; set; }
		[Required]
		public Guid UserId { get; set; }
	}
}
