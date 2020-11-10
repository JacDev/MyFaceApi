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
		private readonly Mock<IUserRepository> _mockRepo;
		private readonly Mock<ILogger<UsersController>> _loggerMock;
		private readonly IMapper _mapper;
		public UsersControllerTests()
		{
			//mocking UserRepo
			_mockRepo = new Mock<IUserRepository>();
			//mocking logger
			_loggerMock = new Mock<ILogger<UsersController>>();
			//mocking automapper
			var myProfile = new UserProfiles();
			var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
			_mapper = new Mapper(configuration);
		}
		[Fact]
		public async void GetUser_ReturnsNotFoundResult_WhenUserDoesntExist()
		{
			//Arrange
			Guid testUserGuid = new Guid("24610263-CEE4-4231-97DF-904EE6437278");

			_mockRepo.Setup(repo => repo.GetUserAsync(testUserGuid))
				.ReturnsAsync(GetTestUserData()
				.FirstOrDefault(
				s => s.Id == testUserGuid));

			var controller = new UsersController(_loggerMock.Object, _mockRepo.Object, _mapper);

			//Act
			var result = await controller.GetUser("97114237-814B-419E-A952-2CE29EBE222F");

			//Assert
			var notFoundResult = Assert.IsType<NotFoundResult>(result.Result);
		}

		[Fact]
		public async void GetUser_ReturnsBadRequestResult_WhenUserIdIsInvalid()
		{
			//Arrange
			var controller = new UsersController(_loggerMock.Object, _mockRepo.Object, _mapper);

			//Act
			var result = await controller.GetUser("InvalidGuid");

			//Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
			Assert.IsType<string>(badRequestResult.Value);
		}
		[Fact]
		public async void GetUser_ReturnsAnActionResult_WithABasicUserData()
		{
			//Arrange
			Guid testUserGuid = new Guid("24610263-CEE4-4231-97DF-904EE6437278");

			_mockRepo.Setup(repo => repo.GetUserAsync(testUserGuid))
				.ReturnsAsync(GetTestUserData()
				.FirstOrDefault(
				s => s.Id == testUserGuid));
			var controller = new UsersController(_loggerMock.Object, _mockRepo.Object, _mapper);
			//Act
			var result = await controller.GetUser(testUserGuid.ToString());

			//Assert
			var actionResult = Assert.IsType<OkObjectResult>(result.Result);
			var model = Assert.IsType<BasicUserData>(actionResult.Value);
			Assert.Equal("Brad", model.FirstName);
			Assert.Equal("Pit", model.LastName);
			Assert.Equal(new Guid("24610263-CEE4-4231-97DF-904EE6437278"), model.Id);
		}
		private List<User> GetTestUserData()
		{
			var database = new List<User>();
			database.Add(new User()
			{
				Id = new Guid("C48D3E36-2072-4A19-9305-FE5168BFB03D"),
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
		[Fact]
		public async void AddUser_ReturnsRedirectToAddedUser_WithBasicUserData()
		{
			//Arrange
			var userToAdd = GetTestUserData().ElementAt(0);

			_mockRepo.Setup(repo => repo.AddUserAcync(userToAdd))
				.ReturnsAsync(userToAdd);
			_mockRepo.Setup(repo => repo.CheckIfUserExists(userToAdd.Id))
				.Returns(false);

			var controller = new UsersController(_loggerMock.Object, _mockRepo.Object, _mapper);
			//Act
			var result = await controller.AddUser(_mapper.Map<BasicUserData>(userToAdd));

			//Assert
			var redirectToActionResult = Assert.IsType<CreatedAtRouteResult>(result.Result);
			Assert.Equal(userToAdd.Id, redirectToActionResult.RouteValues["userId"]);
			Assert.Equal("GetUser", redirectToActionResult.RouteName);
			_mockRepo.Verify();
		}
		[Fact]
		public async void AddUser_ReturnsBadRequest_WhenUserDataAreIncomplete()
		{
			//Arrange
			var userToAdd = new BasicUserData
			{
				FirstName = "Incomplete"
			};

			var controller = new UsersController(_loggerMock.Object, _mockRepo.Object, _mapper);
			//Act
			var result = await controller.AddUser(userToAdd);

			//Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
			Assert.Equal("One or more validation errors occurred.", badRequestResult.Value);
		}
		[Fact]
		public async void AddUser_ReturnsConflict_WhenUserWithGuidAlreadyExist()
		{
			//Arrange
			var userToAdd = GetTestUserData().ElementAt(0);

			_mockRepo.Setup(repo => repo.AddUserAcync(userToAdd))
				.ReturnsAsync(userToAdd);
			_mockRepo.Setup(repo => repo.CheckIfUserExists(userToAdd.Id))
				.Returns(true);

			var controller = new UsersController(_loggerMock.Object, _mockRepo.Object, _mapper);
			//Act
			var result = await controller.AddUser(_mapper.Map<BasicUserData>(userToAdd));

			//Assert
			var conflictRequestResult = Assert.IsType<ConflictObjectResult>(result.Result);
			Assert.Equal($"{userToAdd.Id} already exist.", conflictRequestResult.Value);
		}
	}
}
