﻿using System;
using Xunit;
using System.Linq;
using MyFaceApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyFaceApi.Entities;
using Microsoft.AspNetCore.Http;

namespace MyFaceApi.Tests.UnitTests.UsersControllerTests
{
	public class UsersControllerDeleteUserTests : UsersControllerPreparation
	{
		public UsersControllerDeleteUserTests() : base()
		{
		}
		[Fact]
		public async void DeleteUser_ReturnsNoContentResult_WhenTheUserHasBeenRemoved()
		{
			//Arrange
			var userToRemove = GetTestUserData().ElementAt(0);
			_mockRepo.Setup(repo => repo.GetUserAsync(userToRemove.Id))
				.ReturnsAsync(userToRemove)
				.Verifiable();
			_mockRepo.Setup(repo => repo.DeleteUserAsync(userToRemove))
				.Verifiable();
			var controller = new UsersController(_loggerMock.Object, _mockRepo.Object, _mapper);

			//Act
			var result = await controller.DeleteUser(userToRemove.Id.ToString());

			//Assert
			var noContentResult = Assert.IsType<NoContentResult>(result);
			_mockRepo.Verify();
		}
		[Fact]
		public async void DeleteUser_ReturnsBadRequestResult_WhenTheUserGuidIdIsInvalid()
		{
			//Arrange
			var controller = new UsersController(_loggerMock.Object, _mockRepo.Object, _mapper);

			//Act
			var result = await controller.DeleteUser("InvalidGuid");

			//Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal("InvalidGuid is not valid Guid.", badRequestResult.Value);
		}
		[Fact]
		public async void DeleteUser_ReturnsNotFoundResult_WhenTheUserDoesntExist()
		{
			//Arrange
			Guid testUserGuid = new Guid("24610263-CEE4-4231-97DF-904EE6437278");

			_mockRepo.Setup(repo => repo.GetUserAsync(It.IsAny<Guid>()))
				.ReturnsAsync((User)null)
				.Verifiable();

			var controller = new UsersController(_loggerMock.Object, _mockRepo.Object, _mapper);

			//Act
			var result = await controller.DeleteUser(testUserGuid.ToString());

			//Assert
			var notFoundResult = Assert.IsType<NotFoundResult>(result);
			_mockRepo.Verify();
		}
		[Fact]
		public async void DeleteUser_ReturnsInternalServerError_WhenExceptionThrownInRepository()
		{
			//Arrange
			Guid testUserGuid = new Guid("24610263-CEE4-4231-97DF-904EE6437278");

			_mockRepo.Setup(repo => repo.GetUserAsync(testUserGuid))
				.Throws(new ArgumentNullException(nameof(testUserGuid)))
				.Verifiable();

			var controller = new UsersController(_loggerMock.Object, _mockRepo.Object, _mapper);

			//Act
			var result = await controller.DeleteUser(testUserGuid.ToString());

			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockRepo.Verify();
		}
	}
}