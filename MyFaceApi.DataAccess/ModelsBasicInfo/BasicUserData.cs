using System;
using System.Diagnostics.CodeAnalysis;

namespace MyFaceApi.DataAccess.ModelsBasicInfo
{
	public class BasicUserData
	{
		public Guid Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		[AllowNull]
		public string ProfileImagePath { get; set; }
	}
}
