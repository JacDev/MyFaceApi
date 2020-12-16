using AutoMapper;
using MyFaceApi.Api.Application.DtoModels.Post;

namespace MyFaceApi.Api.Tests.UnitTests.PostsControllerTests
{
	public class PostProfile:Profile
	{
		public PostProfile()
		{
			CreateMap<PostToAddDto, PostDto>();
		}
	}
}
