using MyFaceApi.Api.Application.DtoModels.FriendsRelation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyFaceApi.Api.Application.Interfaces
{
	public interface IFriendsRelationService
	{
		Task<FriendsRelationDto> AddRelationAsync(Guid userId, FriendsRelationToAddDto friendRelation);
		bool CheckIfAreFriends(Guid firstUser, Guid secondUser);
		Task DeleteRelationAsync(Guid userId, Guid friendId);
		FriendsRelationDto GetFriendRelation(Guid firstUser, Guid secondUser);
		List<Guid> GetUserFriendsId(Guid userId);
	}
}
