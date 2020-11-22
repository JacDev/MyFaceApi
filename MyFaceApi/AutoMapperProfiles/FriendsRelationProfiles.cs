using AutoMapper;
using MyFaceApi.DataAccess.Entities;
using MyFaceApi.Models.FriendsRelationModels;

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
