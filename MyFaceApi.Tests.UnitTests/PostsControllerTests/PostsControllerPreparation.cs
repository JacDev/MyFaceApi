using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using Moq;
using MyFaceApi.AutoMapperProfiles;
using MyFaceApi.Api.Controllers;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using MyFaceApi.Api.Application.Interfaces;
using MyFaceApi.Api.Application.FileManagerInterfaces;
using Pagination.Helpers;
using MyFaceApi.Api.Application.DtoModels.Post;

namespace MyFaceApi.Tests.UnitTests.PostsControllerTests
{
	public abstract class PostsControllerPreparation
	{
		protected readonly Mock<IUserService> _mockUserService;
		protected readonly Mock<IPostService> _mockPostService;
		protected readonly Mock<IFriendsRelationService> _mockRelationsService;
		protected readonly Mock<ILogger<PostsController>> _loggerMock;
		protected readonly Mock<IImageManager> _mockImageManager;
		protected readonly PaginationParams _paginationsParams;
		protected readonly IMapper _mapper;
		protected readonly IFixture _fixture;
		protected PostsControllerPreparation()
		{
			//mocking Services
			_mockUserService = new Mock<IUserService>();
			_mockPostService = new Mock<IPostService>();
			_mockRelationsService = new Mock<IFriendsRelationService>();
			//mocking logger
			_loggerMock = new Mock<ILogger<PostsController>>();
			//mocking automapper
			var myProfile = new PostProfiles();
			var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));

			_mockImageManager = new Mock<IImageManager>();
			_mapper = new Mapper(configuration);
			_fixture = new Fixture().Customize(new AutoMoqCustomization());
			_paginationsParams = new PaginationParams()
			{
				PageNumber = 0,
				PageSize = 10,
				Skip = 0
			};
		}
		protected List<PostDto> GetTestPostData()
		{
			return new List<PostDto> {
					new PostDto()
					{
						WhenAdded=DateTime.Now,
						Id = new Guid(ConstIds.ExamplePostId),
						Text="Example text",
						UserId = new Guid(ConstIds.ExampleUserId)
					},
					new PostDto()
					{
						WhenAdded=DateTime.Now,
						Id = new Guid("89248BDB-18C6-420F-A51F-332C3DE4D17C"),
						Text="Example second text",
						UserId = new Guid(ConstIds.ExampleUserId)
					}};
		}
		protected JsonPatchDocument<PostToUpdateDto> GetJsonPatchDocument()
		{
			var jsonobject = new JsonPatchDocument<PostToUpdateDto>
			{
				ContractResolver = new CamelCasePropertyNamesContractResolver()
			};
			jsonobject.Replace(d => d.Text, "Changed");
			return jsonobject;
		}
	}
}
