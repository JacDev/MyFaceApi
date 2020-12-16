using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyFaceApi.Api.Controllers;
using System;
using Xunit;
using AutoFixture;
using MyFaceApi.Api.Application.DtoModels.Notification;

namespace MyFaceApi.Api.Tests.UnitTests.NotificationsControllerTests
{
	public class NotificationsControllerAddNotificationTests : NotificationsControllerPreparation
	{
		protected readonly NotificationToAddDto _notificationToAdd;
		public NotificationsControllerAddNotificationTests() : base()
		{
			_notificationToAdd = _fixture.Create<NotificationToAddDto>();
		}
		[Fact]
		public async void AddNotification_ReturnsCreatedAtRouteResult_WithNotificationData()
		{
			//Arrange
			_mockUserService.Setup(Service => Service.CheckIfUserExists(It.IsAny<Guid>()))
				.ReturnsAsync(true)
				.Verifiable();

			_mockNotificationService.Setup(Service => Service.AddNotificationAsync(It.IsAny<Guid>(), It.IsAny<NotificationToAddDto>()))
				.ReturnsAsync(_mapper.Map<NotificationDto>(_notificationToAdd))
				.Verifiable();

			var controller = new NotificationsController(_loggerMock.Object, _mockNotificationService.Object, _mockUserService.Object);

			//Act
			var result = await controller.AddNotification(ConstIds.ExampleUserId, _notificationToAdd);

			//Assert
			var redirectToActionResult = Assert.IsType<CreatedAtRouteResult>(result.Result);
			Assert.Equal(ConstIds.ExampleUserId, redirectToActionResult.RouteValues["userId"].ToString());
			Assert.NotNull(redirectToActionResult.RouteValues["notificationId"].ToString());
			Assert.Equal("GetNotification", redirectToActionResult.RouteName);
			_mockUserService.Verify();
			_mockNotificationService.Verify();
		}
		[Fact]
		public async void AddNotification_ReturnsBadRequestObjectResult_WhenTheUserIdIsInvalid()
		{
			//Arrange
			var controller = new NotificationsController(_loggerMock.Object, _mockNotificationService.Object, _mockUserService.Object);

			//Act
			var result = await controller.AddNotification(ConstIds.InvalidGuid, _notificationToAdd);

			//Assert
			var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result.Result);
			Assert.Equal($"{ConstIds.InvalidGuid} is not valid guid.", badRequestObjectResult.Value);
		}
		[Fact]
		public async void AddNotification_ReturnsNotFoundObjectResult_WhenTheUserDoesntExist()
		{
			//Arrange
			_mockUserService.Setup(Service => Service.CheckIfUserExists(It.IsAny<Guid>()))
				.ReturnsAsync(false)
				.Verifiable();

			var controller = new NotificationsController(_loggerMock.Object, _mockNotificationService.Object, _mockUserService.Object);

			//Act
			var result = await controller.AddNotification(ConstIds.ExampleUserId, _notificationToAdd);

			//Assert
			var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result.Result);
			Assert.Equal($"User: {ConstIds.ExampleUserId} not found.", notFoundObjectResult.Value);
			_mockUserService.Verify();
		}
		[Fact]
		public async void AddNotification_ReturnsInternalServerErrorResult_WhenExceptionThrownInService()
		{
			//Arrange
			_mockUserService.Setup(Service => Service.CheckIfUserExists(It.IsAny<Guid>()))
				.Throws(new ArgumentNullException(nameof(Guid)))
				.Verifiable();

			var controller = new NotificationsController(_loggerMock.Object, _mockNotificationService.Object, _mockUserService.Object);

			//Act
			var result = await controller.AddNotification(ConstIds.ExampleUserId, _notificationToAdd);
			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result.Result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockUserService.Verify();
		}
	}
}
