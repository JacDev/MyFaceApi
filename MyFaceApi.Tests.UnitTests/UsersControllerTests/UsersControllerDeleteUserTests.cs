using System;
using Xunit;
using System.Linq;
using MyFaceApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyFaceApi.DataAccess.Entities;
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
			var userToRemove = GetTestUserData();
			_mockRepo.Setup(repo => repo.GetUserAsync(It.IsAny<Guid>()))
				.ReturnsAsync(userToRemove)
				.Verifiable();
			_mockRepo.Setup(repo => repo.DeleteUserAsync(It.IsAny<User>()))
				.Verifiable();

			var controller = new UsersController(_loggerMock.Object, _mockRepo.Object, _mapper);

			//Act
			var result = await controller.DeleteUser(ConstIds.ExampleUserId);

			//Assert
			var noContentResult = Assert.IsType<NoContentResult>(result);
			_mockRepo.Verify();
		}
		[Fact]
		public async void DeleteUser_ReturnsBadRequestObjectResult_WhenTheUserGuidIdIsInvalid()
		{
			//Arrange
			var controller = new UsersController(_loggerMock.Object, _mockRepo.Object, _mapper);

			//Act
			var result = await controller.DeleteUser(ConstIds.InvalidGuid);

			//Assert
			var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal($"{ConstIds.InvalidGuid} is not valid Guid.", badRequestObjectResult.Value);
		}
		[Fact]
		public async void DeleteUser_ReturnsNotFoundResult_WhenTheUserDoesntExist()
		{
			//Arrange
			_mockRepo.Setup(repo => repo.GetUserAsync(It.IsAny<Guid>()))
				.ReturnsAsync((User)null)
				.Verifiable();

			var controller = new UsersController(_loggerMock.Object, _mockRepo.Object, _mapper);

			//Act
			var result = await controller.DeleteUser(ConstIds.ExampleUserId);

			//Assert
			var notFoundResult = Assert.IsType<NotFoundResult>(result);
			_mockRepo.Verify();
		}
		[Fact]
		public async void DeleteUser_ReturnsInternalServerErrorResult_WhenExceptionThrownInRepository()
		{
			//Arrange
			_mockRepo.Setup(repo => repo.GetUserAsync(It.IsAny<Guid>()))
				.Throws(new ArgumentNullException(nameof(Guid)))
				.Verifiable();

			var controller = new UsersController(_loggerMock.Object, _mockRepo.Object, _mapper);

			//Act
			var result = await controller.DeleteUser(ConstIds.ExampleUserId);

			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockRepo.Verify();
		}
	}
}
