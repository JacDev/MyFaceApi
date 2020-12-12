using AutoMapper;
using MyFaceApi.Api.Application.DtoModels.User;
using MyFaceApi.Api.Domain.Entities;

namespace MyFaceApi.AutoMapperProfiles
{
	public class UserProfiles : Profile
	{
		public UserProfiles()
		{
			CreateMap<OnlineUser, OnlineUserDto>();
			CreateMap<OnlineUserDto, OnlineUser>();
		}
	}
}
