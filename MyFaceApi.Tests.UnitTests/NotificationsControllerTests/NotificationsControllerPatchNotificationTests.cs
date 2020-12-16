using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyFaceApi.Api.Application.DtoModels.Notification;
using MyFaceApi.Api.Controllers;
using System;
using Xunit;

namespace MyFaceApi.Api.Tests.UnitTests.NotificationsControllerTests
{
	public class NotificationsControllerPatchNotificationTests : NotificationsControllerPreparation
	{
		public NotificationsControllerPatchNotificationTests() : base()
		{
		}
		[Fact]
		public async void PartiallyUpdateNotification_ReturnsNoContentResult_WhenTheNotificationHasBeenUpdated()
		{
			//Arrange
			_mockUserService.Setup(Service => Service.CheckIfUserExists(It.IsAny<Guid>()))
				.ReturnsAsync(true)
				.Verifiable();
			_mockNotificationService.Setup(Service => Service.TryUpdateNotificationAsync(It.IsAny<Guid>(), It.IsAny<JsonPatchDocument<NotificationToUpdateDto>>()))
				.ReturnsAsync(true)
				.Verifiable();

			var controller = new NotificationsController(_loggerMock.Object, _mockNotificationService.Object, _mockUserService.Object);

			//Act
			var result = await controller.PartiallyUpdateNotification(ConstIds.ExampleUserId, ConstIds.ExampleNotificationId, GetJsonPatchDocument());

			//Assert
			var noContentResult = Assert.IsType<NoContentResult>(result);
			_mockUserService.Verify();
			_mockNotificationService.Verify();
		}
		[Fact]
		public async void PartiallyUpdateNotification_ReturnsBadRequestResult_WhenTheNotificationHasNotBeenUpdated()
		{
			//Arrange
			_mockUserService.Setup(Service => Service.CheckIfUserExists(It.IsAny<Guid>()))
				.ReturnsAsync(true)
				.Verifiable();
			_mockNotificationService.Setup(Service => Service.TryUpdateNotificationAsync(It.IsAny<Guid>(), It.IsAny<JsonPatchDocument<NotificationToUpdateDto>>()))
				.ReturnsAsync(false)
				.Verifiable();

			var controller = new NotificationsController(_loggerMock.Object, _mockNotificationService.Object, _mockUserService.Object);

			//Act
			var result = await controller.PartiallyUpdateNotification(ConstIds.ExampleUserId, ConstIds.ExampleNotificationId, GetJsonPatchDocument());

			//Assert
			var badRequestResult = Assert.IsType<BadRequestResult>(result);
			_mockUserService.Verify();
			_mockNotificationService.Verify();
		}
		[Theory]
		[InlineData(ConstIds.InvalidGuid, ConstIds.ExampleNotificationId)]
		[InlineData(ConstIds.ExampleUserId, ConstIds.InvalidGuid)]
		public async void PartiallyUpdateNotification_ReturnsBadRequestObjectResult_WhenUserOrNotificationIdIsInvalid(string testUserId, string testNotificationId)
		{
			//Arrange
			var controller = new NotificationsController(_loggerMock.Object, _mockNotificationService.Object, _mockUserService.Object);

			//Act
			var result = await controller.PartiallyUpdateNotification(testUserId, testNotificationId, GetJsonPatchDocument());

			//Assert
			var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal($"User id: {testUserId} or notification id: {testNotificationId} is not valid guid.", badRequestObjectResult.Value);
		}
		[Fact]
		public async void PartiallyUpdateNotification_NotFoundObjectRequest_WhenTheUserDoesntExist()
		{
			//Arrange
			_mockUserService.Setup(Service => Service.CheckIfUserExists(It.IsAny<Guid>()))
				.ReturnsAsync(false)
				.Verifiable();

			var controller = new NotificationsController(_loggerMock.Object, _mockNotificationService.Object, _mockUserService.Object);

			//Act
			var result = await controller.PartiallyUpdateNotification(ConstIds.ExampleUserId, ConstIds.ExampleNotificationId, GetJsonPatchDocument());

			//Assert
			var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result);

			Assert.Equal($"User: {ConstIds.ExampleUserId} not found.", notFoundObjectResult.Value);
			_mockUserService.Verify();
		}
		[Fact]
		public async void PartiallyUpdateNotification_ReturnsInternalServerErrorResult_WhenExceptionThrownInService()
		{
			//Arrange
			_mockUserService.Setup(Service => Service.CheckIfUserExists(It.IsAny<Guid>()))
				.Throws(new ArgumentNullException(nameof(Guid)))
				.Verifiable();

			var controller = new NotificationsController(_loggerMock.Object, _mockNotificationService.Object, _mockUserService.Object);

			//Act
			var result = await controller.PartiallyUpdateNotification(ConstIds.ExampleUserId, ConstIds.ExampleNotificationId, GetJsonPatchDocument());

			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockUserService.Verify();
		}
	}
}
