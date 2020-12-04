using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using Moq;
using MyFaceApi.AutoMapperProfiles;
using MyFaceApi.Api.Controllers;
using MyFaceApi.Api.DataAccess.Entities;
using MyFaceApi.Api.Models.CommentModels;
using MyFaceApi.Api.Repository.Interfaces;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using MyFaceApi.Api.Repository.Helpers;

namespace MyFaceApi.Tests.UnitTests.PostCommentsControllerTests
{
	public class PostCommentsControllerPreparation
	{
		protected readonly Mock<IPostCommentRepository> _mockCommentRepo;
		protected readonly Mock<IUserRepository> _mockUserRepo;
		protected readonly Mock<IPostRepository> _mockPostRepo;
		protected readonly Mock<ILogger<PostCommentsController>> _loggerMock;
		protected readonly IMapper _mapper;
		protected readonly IFixture _fixture;
		protected readonly PaginationParams _paginationsParams;

		protected PostCommentsControllerPreparation()
		{
			//mocking repos
			_mockCommentRepo = new Mock<IPostCommentRepository>();
			_mockUserRepo = new Mock<IUserRepository>();
			_mockPostRepo = new Mock<IPostRepository>();
			//mocking logger
			_loggerMock = new Mock<ILogger<PostCommentsController>>();
			//mocking automapper
			var myProfile = new CommentProfiles();
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
		protected List<PostComment> GetTestPostData()
		{
			var database = new List<PostComment>()
			{
				new PostComment()
				{
				WhenAdded = DateTime.Now,
				Id = new Guid(ConstIds.ExampleCommentId),
				Text = "Example text",
				FromWho = new Guid(ConstIds.ExampleFromWhoId),
				PostId = new Guid(ConstIds.ExamplePostId)
				},
				new PostComment()
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
		protected JsonPatchDocument<CommentToUpdate> GetJsonPatchDocument()
		{
			var jsonobject = new JsonPatchDocument<CommentToUpdate>
			{
				ContractResolver = new CamelCasePropertyNamesContractResolver()
			};
			jsonobject.Replace(d => d.Text, "Changed");
			return jsonobject;
		}
	}
}
