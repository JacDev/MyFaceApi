using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyFaceApi.Controllers;
using MyFaceApi.Entities;
using MyFaceApi.Models;
using System;
using Xunit;

namespace MyFaceApi.Tests.UnitTests.UsersControllerTests
{
	public class UsersControllerAddUserTests : UsersControllerPreparation
	{
		public UsersControllerAddUserTests() : base()
		{
		}
		[Fact]
		public async void AddUser_ReturnsCreatedAtRouteResult_WithUserData()
		{
			//Arrange
			_mockRepo.Setup(repo => repo.AddUserAcync(It.IsAny<User>()))
				.ReturnsAsync(GetTestUserData())
				.Verifiable();
			_mockRepo.Setup(repo => repo.CheckIfUserExists(It.IsAny<Guid>()))
				.Returns(false)
				.Verifiable();

			var controller = new UsersController(_loggerMock.Object, _mockRepo.Object, _mapper);
			//Act
			var result = await controller.AddUser(_userToAdd);

			//Assert
			var redirectToActionResult = Assert.IsType<CreatedAtRouteResult>(result.Result);
			Assert.Equal(_userToAdd.Id, redirectToActionResult.RouteValues["userId"]);
			Assert.Equal("GetUser", redirectToActionResult.RouteName);
			_mockRepo.Verify();
		}
		[Fact]
		public async void AddUser_ReturnsBadRequestObjectResult_WhenTheUserDataAreIncomplete()
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
			var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result.Result);
			Assert.Equal("One or more validation errors occurred.", badRequestObjectResult.Value);
		}
		[Fact]
		public async void AddUser_ReturnsConflictObjectResult_WhenTheUserWithGuidAlreadyExist()
		{
			//Arrange
			_mockRepo.Setup(repo => repo.CheckIfUserExists(It.IsAny<Guid>()))
				.Returns(true)
				.Verifiable();

			var controller = new UsersController(_loggerMock.Object, _mockRepo.Object, _mapper);
			//Act
			var result = await controller.AddUser(_userToAdd);

			//Assert
			var conflictObjectResult = Assert.IsType<ConflictObjectResult>(result.Result);
			Assert.Equal($"{ConstIds.ExampleUserId} already exist.", conflictObjectResult.Value);
			_mockRepo.Verify();
		}
		[Fact]
		public async void AddUser_ReturnsInternalServerErrorResult_WhenExceptionThrownInRepository()
		{
			//Arrange
			_mockRepo.Setup(repo => repo.CheckIfUserExists(It.IsAny<Guid>()))
				.Throws(new ArgumentNullException(nameof(Guid)))
				.Verifiable();

			var controller = new UsersController(_loggerMock.Object, _mockRepo.Object, _mapper);

			//Act
			var result = await controller.AddUser(_userToAdd);

			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result.Result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockRepo.Verify();
		}
	}
}
