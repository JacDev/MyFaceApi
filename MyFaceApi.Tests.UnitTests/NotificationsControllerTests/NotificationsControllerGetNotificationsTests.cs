using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyFaceApi.Api.Application.DtoModels.Notification;
using MyFaceApi.Api.Controllers;
using System;
using System.Collections.Generic;
using Xunit;

namespace MyFaceApi.Api.Tests.UnitTests.NotificationsControllerTests
{
	public class NotificationsControllerGetNotificationsTests : NotificationsControllerPreparation
	{
		public NotificationsControllerGetNotificationsTests() : base()
		{
		}
		[Fact]
		public void GetNotifications_ReturnsOkObjectResult_WithAListOfNotificationsData()
		{
			//Arrange
			var notifications = GetTestNotificationData();

			_mockUserService.Setup(Service => Service.CheckIfUserExists(It.IsAny<Guid>()))
				.ReturnsAsync(true)
				.Verifiable();
			_mockNotificationService.Setup(Service => Service.GetUserNotifications(It.IsAny<Guid>()))
				.Returns(notifications)
				.Verifiable();

			var controller = new NotificationsController(_loggerMock.Object, _mockNotificationService.Object, _mockUserService.Object);

			//Act
			var result = controller.GetNotifications(ConstIds.ExampleUserId);

			//Assert
			var actionResult = Assert.IsType<OkObjectResult>(result.Result);
			var model = Assert.IsType<List<NotificationDto>>(actionResult.Value);

			Assert.Equal(notifications.Count, model.Count);
			_mockUserService.Verify();
			_mockNotificationService.Verify();
		}
		[Fact]
		public async void GetNotifications_ReturnsBadRequestObjectResult_WhenTheUserIsInvalid()
		{
			//Arrange
			var controller = new NotificationsController(_loggerMock.Object, _mockNotificationService.Object, _mockUserService.Object);

			//Act
			var result = await controller.GetNotifications(ConstIds.InvalidGuid);

			//Assert
			var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result.Result);
			Assert.Equal($"{ConstIds.InvalidGuid} is not valid guid.", badRequestObjectResult.Value);
		}
		[Fact]
		public async void GetNotifications_ReturnsNotFoundObjectResult_WhenUserDoesntExist()
		{
			//Arrange
			_mockUserService.Setup(Service => Service.CheckIfUserExists(It.IsAny<Guid>()))
				.ReturnsAsync(false)
				.Verifiable();

			var controller = new NotificationsController(_loggerMock.Object, _mockNotificationService.Object, _mockUserService.Object);

			//Act
			var result = await controller.GetNotifications(ConstIds.ExampleUserId);
			//Assert
			var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result.Result);

			Assert.Equal($"User: {ConstIds.ExampleUserId} not found.", notFoundObjectResult.Value);
			_mockUserService.Verify();
		}
		[Fact]
		public async void GetNotifications_ReturnsInternalServerErrorResult_WhenExceptionThrownInService()
		{
			//Arrange
			_mockUserService.Setup(Service => Service.CheckIfUserExists(It.IsAny<Guid>()))
				.Throws(new ArgumentNullException(nameof(Guid)))
				.Verifiable();

			var controller = new NotificationsController(_loggerMock.Object, _mockNotificationService.Object, _mockUserService.Object);

			//Act
			var result = await controller.GetNotifications(ConstIds.ExampleUserId);
			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result.Result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockUserService.Verify();
		}
	}
}
