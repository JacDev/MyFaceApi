using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyFaceApi.Controllers;
using MyFaceApi.DataAccess.ModelsBasicInfo;
using System;
using Xunit;

namespace MyFaceApi.Tests.UnitTests.UsersControllerTests
{
	public class UsersControllerGetUserTests : UsersControllerPreparation
	{
		public UsersControllerGetUserTests() : base()
		{
		}
		[Fact]
		public async void GetUser_ReturnsOkObjectResult_WithUserData()
		{
			//Arrange
			var user = GetTestUserData();
			_mockUserRepo.Setup(repo => repo.GetUserAsync(It.IsAny<Guid>()))
				.ReturnsAsync(user)
				.Verifiable();

			var controller = new UsersController(_loggerMock.Object, _mockUserRepo.Object, _mapper);
			//Act
			var result = await controller.GetUser(ConstIds.ExampleUserId);

			//Assert
			var actionResult = Assert.IsType<OkObjectResult>(result.Result);
			var model = Assert.IsType<BasicUserData>(actionResult.Value);
			Assert.Equal(user.FirstName, model.FirstName);
			Assert.Equal(user.LastName, model.LastName);
			Assert.Equal(user.Id, model.Id);
			_mockUserRepo.Verify();
		}
		[Fact]
		public async void GetUser_ReturnsBadRequestObjectResult_WhenTheUserIdIsInvalid()
		{
			//Arrange
			var controller = new UsersController(_loggerMock.Object, _mockUserRepo.Object, _mapper);

			//Act
			var result = await controller.GetUser(ConstIds.InvalidGuid);

			//Assert
			var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result.Result);
			Assert.Equal($"{ConstIds.InvalidGuid} is not valid Guid.", badRequestObjectResult.Value);
		}
		[Fact]
		public async void GetUser_ReturnsInternalServerErrorResult_WhenExceptionThrownInRepository()
		{
			//Arrange
			_mockUserRepo.Setup(repo => repo.GetUserAsync(It.IsAny<Guid>()))
				.Throws(new ArgumentNullException(nameof(Guid)))
				.Verifiable();

			var controller = new UsersController(_loggerMock.Object, _mockUserRepo.Object, _mapper);

			//Act
			var result = await controller.GetUser(ConstIds.ExampleUserId);

			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result.Result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockUserRepo.Verify();
		}
	}
}
