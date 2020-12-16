using AutoMapper;
using MyFaceApi.Api.Application.DtoModels.PostComment;

namespace MyFaceApi.Api.Tests.UnitTests.PostCommentsControllerTests
{
	public class CommentProfile : Profile
	{
		public CommentProfile()
		{
			CreateMap<PostCommentToAddDto, PostCommentDto>();
		}
	}
}
