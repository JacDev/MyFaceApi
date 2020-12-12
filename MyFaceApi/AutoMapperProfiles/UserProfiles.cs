using AutoMapper;
using MyFaceApi.Api.DataAccess.Entities;
using MyFaceApi.Api.DataAccess.ModelsBasicInfo;

namespace MyFaceApi.AutoMapperProfiles
{
	public class UserProfiles : Profile
	{
		public UserProfiles()
		{
			CreateMap<User, BasicUserData>();
			CreateMap<BasicUserData, User>();
		}
	}
}
