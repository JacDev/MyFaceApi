using MyFaceApi.Api.Application.DtoModels.FriendsRelation;
using MyFaceApi.Api.Application.DtoModels.User;
using MyFaceApi.Api.Application.Helpers;
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
		Task<PagedList<UserDto>> GetUserFriends(Guid userId, PaginationParams paginationParams);
		List<Guid> GetUserFriendsId(Guid userId);
	}
}
