using System;
using System.ComponentModel.DataAnnotations;

namespace MyFaceApi.Api.Application.DtoModels.FriendsRelation
{
	public class FriendsRelationToAddDto
	{
		[Required]
		public Guid FriendId { get; set; }
		[Required]
		public DateTime SinceWhen { get; set; }
	}
}
