using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyFaceApi.Api.Application.DtoModels.Notification;
using MyFaceApi.Api.Controllers;
using System;
using System.Linq;
using Xunit;

namespace MyFaceApi.Tests.UnitTests.NotificationsControllerTests
{
	public class NotificationsControllerGetNotificationTests : NotificationsControllerPreparation
	{
		public NotificationsControllerGetNotificationTests() : base()
		{
		}
		[Fact]
		public void GetNotification_ReturnsOkObjectResult_WithNotificationData()
		{
			//Arrange
			var notification = GetTestNotificationData().ElementAt(0);
			_mockUserService.Setup(Service => Service.CheckIfUserExists(It.IsAny<Guid>()))
				.ReturnsAsync(true)
				.Verifiable();
			_mockNotificationService.Setup(Service => Service.GetNotification(It.IsAny<Guid>()))
				.Returns(notification)
				.Verifiable();

			var controller = new NotificationsController(_loggerMock.Object, _mockNotificationService.Object, _mockUserService.Object);

			//Act
			var result = controller.GetNotification(ConstIds.ExampleUserId, ConstIds.ExampleNotificationId);

			//Assert
			var actionResult = Assert.IsType<OkObjectResult>(result.Result);
			var model = Assert.IsType<NotificationDto>(actionResult.Value);
			Assert.Equal(notification, model);
			_mockUserService.Verify();
			_mockNotificationService.Verify();
		}
		[Theory]
		[InlineData(ConstIds.InvalidGuid, ConstIds.ExampleNotificationId)]
		[InlineData(ConstIds.ExampleUserId, ConstIds.InvalidGuid)]
		public async void GetNotification_ReturnsBadRequestObjectResult_WhenUserOrNotificationIdIsInvalid(string testUserId, string testNotificationId)
		{
			//Arrange
			var controller = new NotificationsController(_loggerMock.Object, _mockNotificationService.Object, _mockUserService.Object);

			//Act
			var result = await controller.GetNotification(testUserId, testNotificationId);

			//Assert
			var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result.Result);
			Assert.Equal($"User id: {testUserId} or  notification id: {testNotificationId} is not valid guid.", badRequestObjectResult.Value);
		}
		[Fact]
		public async void GetNotification_ReturnsNotFoundObjectResult_WhenUserDoesntExist()
		{
			//Arrange
			_mockUserService.Setup(Service => Service.CheckIfUserExists(It.IsAny<Guid>()))
				.ReturnsAsync(false)
				.Verifiable();

			var controller = new NotificationsController(_loggerMock.Object, _mockNotificationService.Object, _mockUserService.Object);

			//Act
			var result = await controller.GetNotification(ConstIds.ExampleUserId, ConstIds.ExampleNotificationId);
			//Assert
			var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result.Result);
			Assert.Equal($"User: {ConstIds.ExampleUserId} not found.", notFoundObjectResult.Value);
			_mockUserService.Verify();
		}
		[Fact]
		public async void GetNotification_ReturnsInternalServerErrorResult_WhenExceptionThrownInService()
		{
			//Arrange
			_mockUserService.Setup(Service => Service.CheckIfUserExists(It.IsAny<Guid>()))
				.Throws(new ArgumentNullException(nameof(Guid)))
				.Verifiable();

			var controller = new NotificationsController(_loggerMock.Object, _mockNotificationService.Object, _mockUserService.Object);

			//Act
			var result = await controller.GetNotification(ConstIds.ExampleUserId, ConstIds.ExampleNotificationId);
			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result.Result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockUserService.Verify();
		}
	}
}

