using System;

namespace MyFaceApi.Api.IdentityServer.Application.DtoModels.User
{
	public class IdentityUserDto 
	{
		public Guid Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string ProfileImagePath { get; set; }
		public DateTime DateOfBirht { get; set; }
	}
}
