using AutoMapper;
using MyFaceApi.Api.Application.DtoModels.PostReaction;
using MyFaceApi.Api.Domain.Entities;

namespace MyFaceApi.Api.Application.AutoMapperProfiles
{
	public class ReactionProfiles : Profile
	{
		public ReactionProfiles()
		{
			CreateMap<PostReactionToAddDto, PostReaction>();
			CreateMap<PostReaction, PostReactionDto>();
		}
	}
}
