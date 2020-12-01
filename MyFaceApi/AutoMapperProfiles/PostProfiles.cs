using AutoMapper;
using MyFaceApi.Api.DataAccess.Entities;
using MyFaceApi.Api.Models.PostModels;

namespace MyFaceApi.AutoMapperProfiles
{
	public class PostProfiles : Profile
	{
		public PostProfiles()
		{
			CreateMap<PostToAdd, Post>();
			CreateMap<PostToUpdate, Post>();
			CreateMap<Post, PostToUpdate>();
		}
	}
}
