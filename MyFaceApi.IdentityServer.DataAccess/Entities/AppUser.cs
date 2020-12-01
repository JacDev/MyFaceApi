using Microsoft.AspNetCore.Identity;
using System;
using System.Diagnostics.CodeAnalysis;

namespace MyFaceApi.IdentityServer.DataAccess.Entities
{
	public class AppUser : IdentityUser<Guid>
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		[AllowNull]
		public string ProfileImagePath { get; set; } = null;
		public DateTime DateOfBirht { get; set; }
	}
}
