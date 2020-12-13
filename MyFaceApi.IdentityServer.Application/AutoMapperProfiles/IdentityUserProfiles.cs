using AutoMapper;
using MyFaceApi.Api.IdentityServer.Application.DtoModels.User;
using MyFaceApi.IdentityServer.Domain.Entities;

namespace MyFaceApi.IdentityServer.Application.AutoMapperProfiles
{
	public class IdentityUserProfiles : Profile
	{
		public IdentityUserProfiles()
		{
			CreateMap<ApplicationUser, IdentityUserDto>();
		}
	}
}