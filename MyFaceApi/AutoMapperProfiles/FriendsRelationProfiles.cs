using AutoMapper;
using MyFaceApi.Api.DataAccess.Entities;
using MyFaceApi.Api.Models.FriendsRelationModels;

namespace MyFaceApi.AutoMapperProfiles
{
	public class FriendsRelationProfiles : Profile
	{
		public FriendsRelationProfiles()
		{
			CreateMap<FriendsRelationToAdd, FriendsRelation>();
			CreateMap<FriendsRelation, FriendsRelationToAdd>();
		}
	}
}
