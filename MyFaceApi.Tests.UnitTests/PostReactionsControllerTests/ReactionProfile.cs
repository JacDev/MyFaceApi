using AutoMapper;
using MyFaceApi.Api.Application.DtoModels.PostReaction;

namespace MyFaceApi.Api.Tests.UnitTests.PostReactionsControllerTests
{
	public class ReactionProfile : Profile
	{
		public ReactionProfile()
		{
			CreateMap<PostReactionToAddDto, PostReactionDto>();
		}
	}
}
