using AutoMapper;
using MyFaceApi.Api.Application.DtoModels.FriendsRelation;

namespace MyFaceApi.Api.Tests.UnitTests.FriendsRelationsControllerTests
{
	public class FriendsRelationProfile : Profile
	{
		public FriendsRelationProfile()
		{
			CreateMap<FriendsRelationToAddDto, FriendsRelationDto>();
		}
	}
}
