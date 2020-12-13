using Microsoft.AspNetCore.Identity;
using System;

namespace MyFaceApi.IdentityServer.Domain.Entities
{
	public class ApplicationUser: IdentityUser<Guid>
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string ProfileImagePath { get; set; }
		public DateTime DateOfBirht { get; set; }
	}
}
