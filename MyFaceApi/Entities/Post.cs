using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyFaceApi.Entities
{
	public class Post
	{
		[Key]
		public Guid Id { get; set; }
		public DateTime WhenAdded { get; set; }
		[Required]
		public string Text { get; set; }
		public Guid UserId { get; set; }
		public string ImagePath { get; set; }
		public string ImageFullPath { get; set; }
	}
}
