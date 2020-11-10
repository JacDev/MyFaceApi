using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyFaceApi.Models
{
	public class BasicUserData
	{
		[Required]
		public Guid Id { get; set; }
		[Required]
		public string FirstName { get; set; }
		[Required]
		public string LastName { get; set; }
		public string ProfileImagePath { get; set; } = null;
	}
}
