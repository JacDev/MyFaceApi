using System;

namespace MyFaceApi.Api.Domain.Entities
{
	public class FriendsRelation
	{
		public Guid UserId { get; set; }
		public Guid FriendId { get; set; }
		public DateTime SinceWhen { get; set; }
	}
}
