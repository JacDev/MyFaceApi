using AutoMapper;
using MyFaceApi.Api.DataAccess.ModelsBasicInfo;
using MyFaceApi.IdentityServer.DataAccess.Entities;

namespace MyFaceApi.IdentityServer.AutoMapperProfiles
{
	public class UserProfiles : Profile
	{
		public UserProfiles()
		{
			CreateMap<AppUser, BasicUserData>();
		}
	}
}
