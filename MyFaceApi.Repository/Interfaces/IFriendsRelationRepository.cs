using MyFaceApi.Api.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyFaceApi.Api.Repository.Interfaces
{
	public interface IFriendsRelationRepository
	{
		List<FriendsRelation> GetUserRelationships(Guid userId);
		FriendsRelation GetFriendRelation(Guid firstUser, Guid secondUser);
		Task<FriendsRelation> AddRelationAsync(FriendsRelation friendRelation);
		Task DeleteRelationAsync(FriendsRelation friendRelation);
		bool CheckIfFriends(Guid firstUser, Guid secondUser);
	}
}
