using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace MyFaceApi.Api.DataAccess.Entities
{
	public class OnlineUserModel
	{
		[NotNull]
		public string Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		[AllowNull]
		public string ConnectionId { get; set; } = string.Empty;
	}
}
