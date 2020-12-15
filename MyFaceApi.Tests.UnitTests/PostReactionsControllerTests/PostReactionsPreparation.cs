using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using Moq;
using MyFaceApi.Api.Application.DtoModels.PostReaction;
using MyFaceApi.Api.Application.Interfaces;
using MyFaceApi.Api.Controllers;
using MyFaceApi.Api.Domain.Enums;
using MyFaceApi.AutoMapperProfiles;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;

namespace MyFaceApi.Tests.UnitTests.PostReactionsControllerTests
{
	public class PostReactionsPreparation
	{
		protected readonly Mock<IUserService> _mockUserService;
		protected readonly Mock<IPostService> _mockPostService;
		protected readonly Mock<IPostReactionService> _mockReactionService;
		protected readonly Mock<ILogger<PostReactionsController>> _loggerMock;
		protected readonly IMapper _mapper;
		protected readonly IFixture _fixture;
		protected PostReactionsPreparation()
		{
			//mocking Services
			_mockUserService = new Mock<IUserService>();
			_mockPostService = new Mock<IPostService>();
			_mockReactionService = new Mock<IPostReactionService>();
			//mocking logger
			_loggerMock = new Mock<ILogger<PostReactionsController>>();
			//mocking automapper
			var myProfile = new ReactionProfiles();
			var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
			_mapper = new Mapper(configuration);
			_fixture = new Fixture().Customize(new AutoMoqCustomization());
		}
		protected List<PostReactionDto> GetTestPostData()
		{
			var reactions = new List<PostReactionDto>()
			{
					new PostReactionDto
					{
						Id = new Guid(ConstIds.ExampleReactionId),
						WhenAdded = DateTime.Now,
						FromWho = new Guid(ConstIds.ExampleFromWhoId),
						PostId = new Guid(ConstIds.ExamplePostId),
						Reaction = ReactionType.Like

					},
					new PostReactionDto
					{
						Id = new Guid("2D748217-9B12-4F56-831D-F2D3A6A3B074"),
						WhenAdded = DateTime.Now,
						FromWho = new Guid("4CEEB904-E88C-46AD-853D-0DADCF3C7C29"),
						PostId = new Guid(ConstIds.ExamplePostId),
						Reaction = ReactionType.Like
					}
			};
			return reactions;
		}
		protected JsonPatchDocument<PostReactionToUpdateDto> GetJsonPatchDocument()
		{
			var jsonobject = new JsonPatchDocument<PostReactionToUpdateDto>
			{
				ContractResolver = new CamelCasePropertyNamesContractResolver()
			};
			jsonobject.Replace(d => d.Reaction, ReactionType.Haha);
			return jsonobject;
		}
	}
}
