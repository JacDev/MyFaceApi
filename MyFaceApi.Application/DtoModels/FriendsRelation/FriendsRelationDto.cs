using System;

namespace MyFaceApi.Api.Application.DtoModels.FriendsRelation
{
	public class FriendsRelationDto
	{
		public Guid UserId { get; set; }
		public Guid FriendId { get; set; }
		public DateTime SinceWhen { get; set; }
	}
}
