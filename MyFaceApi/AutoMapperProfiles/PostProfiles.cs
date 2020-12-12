using AutoMapper;
using MyFaceApi.Api.DataAccess.Entities;
using MyFaceApi.Api.DboModels;
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
			CreateMap<Post, PostDbo>()
				.ForMember(
				dest => dest.PostCommentsCounter,
				opt => opt.MapFrom(src => src.PostComments.Count))
				.ForMember(
				dest => dest.PostReactionsCounter,
				opt => opt.MapFrom(src => src.PostReactions.Count));
		}
	}
}
