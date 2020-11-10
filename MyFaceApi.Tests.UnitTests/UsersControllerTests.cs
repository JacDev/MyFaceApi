using System;
using Xunit;
using Moq;
using MyFaceApi.Repository;
using MyFaceApi.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MyFaceApi.Entities;
using MyFaceApi.Controllers;
using Microsoft.Extensions.Logging;
using MyFaceApi.AutoMapperProfiles;
using AutoMapper;

namespace MyFaceApi.Tests.UnitTests
{
	public class UsersControllerTests
	{
		[Fact]
		public async void GetUser_ReturnsNotFoundResult_WhenUserDoesntExist()
		{
			//Arrange
			var mockRepo = new Mock<IUserRepository>();
			Guid testUserGuid = new Guid("24610263-CEE4-4231-97DF-904EE6437278");

			mockRepo.Setup(repo => repo.GetUserAsync(testUserGuid))
				.ReturnsAsync(GetTestUserData()
				.FirstOrDefault(
				s => s.Id == testUserGuid));

			var loggerMock = new Mock<ILogger<UsersController>>();
			var myProfile = new UserProfiles();
			var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
			IMapper mapper = new Mapper(configuration);
			var controller = new UsersController(loggerMock.Object, mockRepo.Object, mapper);

			//Act
			var result = await controller.GetUser("97114237-814B-419E-A952-2CE29EBE222F");

			//Assert
			var notFoundResult = Assert.IsType<NotFoundResult>(result.Result);
		}

		[Fact]
		public async void GetUser_ReturnsBadRequestResult_WhenUserIdIsInvalid()
		{
			//Arrange
			var mockRepo = new Mock<IUserRepository>();

			var loggerMock = new Mock<ILogger<UsersController>>();
			var myProfile = new UserProfiles();
			var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
			IMapper mapper = new Mapper(configuration);
			var controller = new UsersController(loggerMock.Object, mockRepo.Object, mapper);

			//Act
			var result = await controller.GetUser("InvalidGuid");

			//Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
			Assert.IsType<string>(badRequestResult.Value);
		}
		[Fact]
		public async void GetUser_ReturnsAnActionResult_WithAUserToReturn()
		{
			//Arrange
			//mocking userrepository
			var mockRepo = new Mock<IUserRepository>();
			Guid testUserGuid = new Guid("24610263-CEE4-4231-97DF-904EE6437278");

			mockRepo.Setup(repo => repo.GetUserAsync(testUserGuid))
				.ReturnsAsync(GetTestUserData()
				.FirstOrDefault(
				s => s.Id == testUserGuid));
			//mocking logger
			var loggerMock = new Mock<ILogger<UsersController>>();
			//mocking automapper
			var myProfile = new UserProfiles();
			var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
			IMapper mapper = new Mapper(configuration);

			var controller = new UsersController(loggerMock.Object, mockRepo.Object, mapper);
			//Act
			var result = await controller.GetUser(testUserGuid.ToString());

			//Assert
			var actionResult = Assert.IsType<OkObjectResult>(result.Result);
			var model = Assert.IsType<UserToReturn>(actionResult.Value);
			Assert.Equal("Brad", model.FirstName);
			Assert.Equal("Pit", model.LastName);
			Assert.Equal(new Guid("24610263-CEE4-4231-97DF-904EE6437278"), model.Id);
		}
		private List<User> GetTestUserData()
		{
			var database = new List<User>();
			database.Add(new User()
			{
				Id = new Guid(),
				FirstName = "Mark",
				LastName = "Twain",
				ProfileImagePath = null,
			});
			database.Add(new User()
			{
				Id = new Guid("24610263-CEE4-4231-97DF-904EE6437278"),
				FirstName = "Brad",
				LastName = "Pit"
			});
			return database;
		}
	}
}
