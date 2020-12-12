using AutoMapper;
using MyFaceApi.Api.Application.DtoModels.FriendsRelation;
using MyFaceApi.Api.Domain.Entities;

namespace MyFaceApi.AutoMapperProfiles
{
	public class FriendsRelationProfiles : Profile
	{
		public FriendsRelationProfiles()
		{
			CreateMap<FriendsRelationToAddDto, FriendsRelation>();
			CreateMap<FriendsRelation, FriendsRelationToAddDto>();
		}
	}
}
