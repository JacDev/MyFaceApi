using AutoMapper;
using MyFaceApi.Api.Application.DtoModels.User;
using MyFaceApi.Api.Domain.Entities;

namespace MyFaceApi.Api.Application.AutoMapperProfiles
{
	public class UserProfiles : Profile
	{
		public UserProfiles()
		{
			CreateMap<OnlineUserDto, OnlineUser>();
			CreateMap<OnlineUser, OnlineUserDto>();
		}
	}
}
