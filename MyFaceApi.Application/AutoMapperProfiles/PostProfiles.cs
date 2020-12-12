using AutoMapper;
using MyFaceApi.Api.Application.DtoModels.Post;
using MyFaceApi.Api.Domain.Entities;

namespace MyFaceApi.AutoMapperProfiles
{
	public class PostProfiles : Profile
	{
		public PostProfiles()
		{
			CreateMap<PostToAddDto, Post>();
			CreateMap<PostToUpdateDto, Post>();
			CreateMap<Post, PostDto>()
				.ForMember(
				dest => dest.PostCommentsCounter,
				opt => opt.MapFrom(src => src.PostComments.Count))
				.ForMember(
				dest => dest.PostReactionsCounter,
				opt => opt.MapFrom(src => src.PostReactions.Count));
		}
	}
}
