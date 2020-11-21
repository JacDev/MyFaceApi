using AutoMapper;
using MyFaceApi.DataAccess.Entities;
using MyFaceApi.Models.PostModels;

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
