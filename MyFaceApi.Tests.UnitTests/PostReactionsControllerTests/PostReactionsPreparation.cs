using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using Moq;
using MyFaceApi.AutoMapperProfiles;
using MyFaceApi.Controllers;
using MyFaceApi.DataAccess.Entities;
using MyFaceApi.Models.PostReactionModels;
using MyFaceApi.Repository.Interfaces;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;

namespace MyFaceApi.Tests.UnitTests.PostReactionsControllerTests
{
	public class PostReactionsPreparation
	{
		protected readonly Mock<IUserRepository> _mockUserRepo;
		protected readonly Mock<IPostRepository> _mockPostRepo;
		protected readonly Mock<IPostReactionRepository> _mockReactionRepo;
		protected readonly Mock<ILogger<PostReactionsController>> _loggerMock;
		protected readonly IMapper _mapper;
		protected readonly PostReactionToAdd _postReactionToAdd;
		protected PostReactionsPreparation()
		{
			//mocking repos
			_mockUserRepo = new Mock<IUserRepository>();
			_mockPostRepo = new Mock<IPostRepository>();
			_mockReactionRepo = new Mock<IPostReactionRepository>();
			//mocking logger
			_loggerMock = new Mock<ILogger<PostReactionsController>>();
			//mocking automapper
			var myProfile = new ReactionProfiles();
			var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
			_mapper = new Mapper(configuration);
			_postReactionToAdd = new PostReactionToAdd
			{
				WhenAdded = DateTime.Now,
				FromWho = new Guid(ConstIds.ExampleFromWhoId),
				Reaction = DataAccess.Enums.ReactionType.Like
			};
		}
		protected Post GetTestPostData()
		{
			var post = new Post()
			{
				WhenAdded = DateTime.Now,
				Id = new Guid(ConstIds.ExamplePostId),
				Text = "Example text",
				UserId = new Guid(ConstIds.ExampleUserId),
				PostReactions = new List<PostReaction>
				{
					new PostReaction
					{
						Id = new Guid(ConstIds.ExampleReactionId),
						WhenAdded = DateTime.Now,
						FromWho = new Guid(ConstIds.ExampleFromWhoId),
						PostId = new Guid(ConstIds.ExamplePostId),
						Reaction = DataAccess.Enums.ReactionType.Like

					},
					new PostReaction
					{
						Id = new Guid("2D748217-9B12-4F56-831D-F2D3A6A3B074"),
						WhenAdded = DateTime.Now,
						FromWho = new Guid("4CEEB904-E88C-46AD-853D-0DADCF3C7C29"),
						PostId = new Guid(ConstIds.ExamplePostId),
						Reaction = DataAccess.Enums.ReactionType.Like
					}
				}
			};
			return post;
		}
		protected JsonPatchDocument<PostReactionToUpdate> GetJsonPatchDocument()
		{
			var jsonobject = new JsonPatchDocument<PostReactionToUpdate>
			{
				ContractResolver = new CamelCasePropertyNamesContractResolver()
			};
			jsonobject.Replace(d => d.Reaction, DataAccess.Enums.ReactionType.Haha);
			return jsonobject;
		}
	}
}
