using MyFaceApi.DataAccess.ModelsBasicInfo;
using System;

namespace MyFaceApi.DataAccess.Entities
{
	public class FriendsRelation : BasicFriendsRelationData
	{
		public Guid UserId { get; set; }
	}
}
