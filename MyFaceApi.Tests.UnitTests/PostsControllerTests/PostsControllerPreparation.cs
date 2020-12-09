using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using Moq;
using MyFaceApi.AutoMapperProfiles;
using MyFaceApi.Api.Controllers;
using MyFaceApi.Api.DataAccess.Entities;
using MyFaceApi.Api.Models.PostModels;
using MyFaceApi.Api.Repository.Interfaces;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using MyFaceApi.Api.Repository.Helpers;
using MyFaceApi.Api.FileManager;

namespace MyFaceApi.Tests.UnitTests.PostsControllerTests
{
	public abstract class PostsControllerPreparation
	{
		protected readonly Mock<IUserRepository> _mockUserRepo;
		protected readonly Mock<IPostRepository> _mockPostRepo;
		protected readonly Mock<IFriendsRelationRepository> _mockRelationsRepo;
		protected readonly Mock<ILogger<PostsController>> _loggerMock;
		protected readonly Mock<IImageManager> _mockImageManager;
		protected readonly PaginationParams _paginationsParams;
		protected readonly IMapper _mapper;
		protected readonly IFixture _fixture;
		protected PostsControllerPreparation()
		{
			//mocking repos
			_mockUserRepo = new Mock<IUserRepository>();
			_mockPostRepo = new Mock<IPostRepository>();
			_mockRelationsRepo = new Mock<IFriendsRelationRepository>();
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
		protected List<User> GetTestUserData()
		{
			var database = new List<User>
			{
				new User()
				{
					Id = new Guid(ConstIds.ExampleUserId),
					FirstName = "Mark",
					LastName = "Twain",
					ProfileImagePath = null,
					Posts = new List<Post>
					{
						new Post()
						{
							WhenAdded=DateTime.Now,
							Id = new Guid(ConstIds.ExamplePostId),
							Text="Example text",
							UserId = new Guid(ConstIds.ExampleUserId)
						},
						new Post()
						{
							WhenAdded=DateTime.Now,
							Id = new Guid("89248BDB-18C6-420F-A51F-332C3DE4D17C"),
							Text="Example second text",
							UserId = new Guid(ConstIds.ExampleUserId)
						},
					}
				},
				new User()
				{
					Id = new Guid("24610263-CEE4-4231-97DF-904EE6437278"),
					FirstName = "Brad",
					LastName = "Pit"
				}
			};
			return database;
		}
		protected JsonPatchDocument<PostToUpdate> GetJsonPatchDocument()
		{
			var jsonobject = new JsonPatchDocument<PostToUpdate>
			{
				ContractResolver = new CamelCasePropertyNamesContractResolver()
			};
			jsonobject.Replace(d => d.Text, "Changed");
			return jsonobject;
		}
	}
}
