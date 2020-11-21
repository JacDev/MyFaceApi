using AutoMapper;
using MyFaceApi.DataAccess.Entities;
using MyFaceApi.DataAccess.ModelsBasicInfo;

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
