using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using Moq;
using MyFaceApi.Api.Controllers;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using MyFaceApi.Api.Application.Interfaces;
using Pagination.Helpers;
using MyFaceApi.Api.Application.DtoModels.PostComment;

namespace MyFaceApi.Api.Tests.UnitTests.PostCommentsControllerTests
{
	public class PostCommentsControllerPreparation
	{
		protected readonly Mock<IPostCommentService> _mockCommentService;
		protected readonly Mock<IUserService> _mockUserService;
		protected readonly Mock<IPostService> _mockPostService;
		protected readonly Mock<ILogger<PostCommentsController>> _loggerMock;
		protected readonly IMapper _mapper;
		protected readonly IFixture _fixture;
		protected readonly PaginationParams _paginationsParams;

		protected PostCommentsControllerPreparation()
		{
			//mocking Services
			_mockCommentService = new Mock<IPostCommentService>();
			_mockUserService = new Mock<IUserService>();
			_mockPostService = new Mock<IPostService>();
			//mocking logger
			_loggerMock = new Mock<ILogger<PostCommentsController>>();
			//mocking automapper
			var myProfile = new CommentProfile();
			var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
			_mapper = new Mapper(configuration);
			_fixture = new Fixture().Customize(new AutoMoqCustomization());
			_paginationsParams = new PaginationParams()
			{
				PageNumber = 0,
				PageSize = 10,
				Skip = 0
			};
		}
		protected List<PostCommentDto> GetTestPostData()
		{
			var database = new List<PostCommentDto>()
			{
				new PostCommentDto()
				{
				WhenAdded = DateTime.Now,
				Id = new Guid(ConstIds.ExampleCommentId),
				Text = "Example text",
				FromWho = new Guid(ConstIds.ExampleFromWhoId),
				PostId = new Guid(ConstIds.ExamplePostId)
				},
				new PostCommentDto()
				{
				WhenAdded = DateTime.Now,
				Id = new Guid("A5A276EE-4936-4162-A2AC-880E525BD992"),
				Text = "Example text",
				FromWho = new Guid(ConstIds.ExampleFromWhoId),
				PostId = new Guid(ConstIds.ExamplePostId)
				}
			};
			return database;
		}
		protected JsonPatchDocument<PostCommentToUpdateDto> GetJsonPatchDocument()
		{
			var jsonobject = new JsonPatchDocument<PostCommentToUpdateDto>
			{
				ContractResolver = new CamelCasePropertyNamesContractResolver()
			};
			jsonobject.Replace(d => d.Text, "Changed");
			return jsonobject;
		}
	}
}
