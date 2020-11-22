using MyFaceApi.DataAccess.ModelsBasicInfo;
using System;

namespace MyFaceApi.DataAccess.Entities
{
	public class FriendsRelation : BasicFriendsRelationInfo
	{
		public Guid UserId { get; set; }
	}
}
