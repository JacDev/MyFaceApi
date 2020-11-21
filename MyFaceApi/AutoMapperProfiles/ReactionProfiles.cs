using AutoMapper;
using MyFaceApi.DataAccess.Entities;
using MyFaceApi.Models.PostReactionModels;

namespace MyFaceApi.AutoMapperProfiles
{
	public class ReactionProfiles : Profile
	{
		public ReactionProfiles()
		{
			CreateMap<PostReaction, PostReactionToAdd>();
			CreateMap<PostReactionToAdd, PostReaction>();
			CreateMap<PostReactionToUpdate, PostReaction>();
			CreateMap<PostReaction, PostReactionToUpdate>();
		}
	}
}
