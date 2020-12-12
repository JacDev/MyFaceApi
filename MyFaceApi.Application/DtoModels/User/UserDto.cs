using System;

namespace MyFaceApi.Api.Application.DtoModels.User
{
	public class UserDto 
	{
		public Guid Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string ProfileImagePath { get; set; }
		public DateTime DateOfBirht { get; set; }
		public int NewNotificationsCounter { get; set; }
		public int TotalFriendsCounter { get; set; }
		public int TotalPostCounter { get; set; }
	}
}
