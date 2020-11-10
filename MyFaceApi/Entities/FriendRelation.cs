using System;

namespace MyFaceApi.Entities
{
	public class FriendRelation
	{
		public Guid UserId { get; set; }
		public Guid FriendId { get; set; }
		public DateTime SinceWhen { get; set; }
	}
}
