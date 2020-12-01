using MyFaceApi.Api.DataAccess.ModelsBasicInfo;
using System;

namespace MyFaceApi.Api.DataAccess.Entities
{
	public class FriendsRelation : BasicFriendsRelationData
	{
		public Guid UserId { get; set; }
	}
}
