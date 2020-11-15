using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyFaceApi.Controllers;
using MyFaceApi.Entities;
using MyFaceApi.Models;
using System;
using System.Linq;
using Xunit;

namespace MyFaceApi.Tests.UnitTests.UsersControllerTests
{
	public class UsersControllerGetUserTests : UsersControllerPreparation
	{
		public UsersControllerGetUserTests() : base()
		{
		}
		[Fact]
		public async void GetUser_ReturnsAnActionResult_WithABasicUserData()
		{
			//Arrange
			Guid testUserGuid = new Guid("24610263-CEE4-4231-97DF-904EE6437278");

			_mockRepo.Setup(repo => repo.GetUserAsync(testUserGuid))
				.ReturnsAsync(GetTestUserData()
				.FirstOrDefault(
				s => s.Id == testUserGuid))
				.Verifiable();

			var controller = new UsersController(_loggerMock.Object, _mockRepo.Object, _mapper);
			//Act
			var result = await controller.GetUser(testUserGuid.ToString());

			//Assert
			var actionResult = Assert.IsType<OkObjectResult>(result.Result);
			var model = Assert.IsType<BasicUserData>(actionResult.Value);
			Assert.Equal("Brad", model.FirstName);
			Assert.Equal("Pit", model.LastName);
			Assert.Equal(new Guid("24610263-CEE4-4231-97DF-904EE6437278"), model.Id);
			_mockRepo.Verify();
		}
		[Fact]
		public async void GetUser_ReturnsBadRequestResult_WhenTheUserIdIsInvalid()
		{
			//Arrange
			var controller = new UsersController(_loggerMock.Object, _mockRepo.Object, _mapper);

			//Act
			var result = await controller.GetUser("InvalidGuid");

			//Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
			Assert.Equal("InvalidGuid is not valid Guid.", badRequestResult.Value);
		}
		[Fact]
		public async void GetUser_ReturnsNotFoundResult_WhenTheUserDoesntExist()
		{
			//Arrange
			Guid testUserGuid = new Guid("24610263-CEE4-4231-97DF-904EE6437278");

			_mockRepo.Setup(repo => repo.GetUserAsync(testUserGuid))
				.ReturnsAsync((User)null)
				.Verifiable();

			var controller = new UsersController(_loggerMock.Object, _mockRepo.Object, _mapper);

			//Act
			var result = await controller.GetUser(testUserGuid.ToString());

			//Assert
			var notFoundResult = Assert.IsType<NotFoundResult>(result.Result);
			_mockRepo.Verify();
		}
		[Fact]
		public async void GetUser_ReturnsInternalServerError_WhenExceptionThrownInRepository()
		{
			//Arrange
			Guid testUserGuid = new Guid("24610263-CEE4-4231-97DF-904EE6437278");

			_mockRepo.Setup(repo => repo.GetUserAsync(testUserGuid))
				.Throws(new ArgumentNullException(nameof(testUserGuid)))
				.Verifiable();

			var controller = new UsersController(_loggerMock.Object, _mockRepo.Object, _mapper);

			//Act
			var result = await controller.GetUser("24610263-CEE4-4231-97DF-904EE6437278");

			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result.Result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockRepo.Verify();
		}
	}
}
