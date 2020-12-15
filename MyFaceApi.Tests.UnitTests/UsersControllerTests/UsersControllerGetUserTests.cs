using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using MyFaceApi.Api.Application.DtoModels.User;
using MyFaceApi.Api.Application.Interfaces;
using MyFaceApi.Api.Controllers;
using System;
using Xunit;

namespace MyFaceApi.Tests.UnitTests.UsersControllerTests
{
	public class UsersControllerGetUserTests
	{
		protected readonly Mock<IUserService> _mockUserService;
		protected readonly Mock<ILogger<UsersController>> _loggerMock;
		protected readonly IFixture _fixture;
		public UsersControllerGetUserTests()
		{
			//mocking UserService
			_mockUserService = new Mock<IUserService>();
			//mocking logger
			_loggerMock = new Mock<ILogger<UsersController>>();
			_fixture = new Fixture().Customize(new AutoMoqCustomization());
		}
		protected UserDto GetTestUserData()
		{
			return _fixture.Create<UserDto>();
		}
		[Fact]
		public async void GetUser_ReturnsOkObjectResult_WithUserDDto()
		{
			//Arrange
			var user = GetTestUserData();
			_mockUserService.Setup(Service => Service.GetUserAsync(It.IsAny<Guid>()))
				.ReturnsAsync(user)
				.Verifiable();

			var controller = new UsersController(_loggerMock.Object, _mockUserService.Object);
			//Act
			var result = await controller.GetUser(ConstIds.ExampleUserId);

			//Assert
			var actionResult = Assert.IsType<OkObjectResult>(result.Result);
			var model = Assert.IsType<UserDto>(actionResult.Value);
			Assert.Equal(user.FirstName, model.FirstName);
			Assert.Equal(user.LastName, model.LastName);
			Assert.Equal(user.Id, model.Id);
			_mockUserService.Verify();
		}
		[Fact]
		public async void GetUser_ReturnsBadRequestObjectResult_WhenTheUserIdIsInvalid()
		{
			//Arrange
			var controller = new UsersController(_loggerMock.Object, _mockUserService.Object);

			//Act
			var result = await controller.GetUser(ConstIds.InvalidGuid);

			//Assert
			var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result.Result);
			Assert.Equal($"{ConstIds.InvalidGuid} is not valid guid.", badRequestObjectResult.Value);
		}
		[Fact]
		public async void GetUser_ReturnsInternalServerErrorResult_WhenExceptionThrownInService()
		{
			//Arrange
			_mockUserService.Setup(Service => Service.GetUserAsync(It.IsAny<Guid>()))
				.Throws(new ArgumentNullException(nameof(Guid)))
				.Verifiable();

			var controller = new UsersController(_loggerMock.Object, _mockUserService.Object);

			//Act
			var result = await controller.GetUser(ConstIds.ExampleUserId);

			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result.Result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockUserService.Verify();
		}
	}
}
