using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using MyFaceApi.AutoMapperProfiles;
using MyFaceApi.Controllers;
using MyFaceApi.Entities;
using MyFaceApi.Repository.Interfaceses;
using System;
using System.Collections.Generic;

namespace MyFaceApi.Tests.UnitTests.PostsControllerTests
{
	public abstract class PostsControllerPreparation
	{
		protected readonly Mock<IUserRepository> _mockUserRepo;
		protected readonly Mock<IPostRepository> _mockPostRepo;
		protected readonly Mock<ILogger<PostsController>> _loggerMock;
		protected readonly IMapper _mapper;
		protected PostsControllerPreparation()
		{
			//mocking repos
			_mockUserRepo = new Mock<IUserRepository>();
			_mockPostRepo = new Mock<IPostRepository>();
			//mocking logger
			_loggerMock = new Mock<ILogger<PostsController>>();
			//mocking automapper
			var myProfile = new PostProfiles();
			var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
			_mapper = new Mapper(configuration);
		}
		protected List<User> GetTestUserData()
		{
			var database = new List<User>
			{
				new User()
				{
					Id = new Guid("C48D3E36-2072-4A19-9305-FE5168BFB03D"),
					FirstName = "Mark",
					LastName = "Twain",
					ProfileImagePath = null,
					Posts = new List<Post>
					{
						new Post()
						{
							WhenAdded=DateTime.Now,
							Id = new Guid("9B92C388-C51D-4EF5-997F-A40F3AFB7E9D"),
							Text="Example text",
							UserId = new Guid("C48D3E36-2072-4A19-9305-FE5168BFB03D")
						},
						new Post()
						{
							WhenAdded=DateTime.Now,
							Id = new Guid("89248BDB-18C6-420F-A51F-332C3DE4D17C"),
							Text="Example second text",
							UserId = new Guid("C48D3E36-2072-4A19-9305-FE5168BFB03D")
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
	}
}
