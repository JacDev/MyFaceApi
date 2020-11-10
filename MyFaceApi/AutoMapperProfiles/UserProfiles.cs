using AutoMapper;
using MyFaceApi.Entities;
using MyFaceApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
