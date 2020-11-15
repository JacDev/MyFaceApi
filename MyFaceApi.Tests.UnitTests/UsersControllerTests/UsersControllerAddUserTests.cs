using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyFaceApi.Controllers;
using MyFaceApi.Models;
using System;
using System.Linq;
using Xunit;

namespace MyFaceApi.Tests.UnitTests.UsersControllerTests
{
	public class UsersControllerAddUserTests : UsersControllerPreparation
	{
		public UsersControllerAddUserTests() : base()
		{
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
		public async void AddUser_ReturnsBadRequest_WhenTheUserDataAreIncomplete()
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
			_mockRepo.Verify();
		}
		[Fact]
		public async void AddUser_ReturnsConflict_WhenTheUserWithGuidAlreadyExist()
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
			_mockRepo.Verify();
		}
		[Fact]
		public async void AddUser_ReturnsInternalServerError_WhenExceptionThrownInRepository()
		{
			//Arrange
			var userToAdd = GetTestUserData().ElementAt(0);
			_mockRepo.Setup(repo => repo.CheckIfUserExists(userToAdd.Id))
				.Throws(new ArgumentNullException(nameof(userToAdd.Id)));

			var controller = new UsersController(_loggerMock.Object, _mockRepo.Object, _mapper);

			//Act
			var result = await controller.AddUser(_mapper.Map<BasicUserData>(userToAdd));

			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result.Result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockRepo.Verify();
		}
	}
}
