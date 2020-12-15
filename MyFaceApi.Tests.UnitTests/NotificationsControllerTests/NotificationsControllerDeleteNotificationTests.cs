using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyFaceApi.Api.Controllers;
using System;
using Xunit;

namespace MyFaceApi.Tests.UnitTests.NotificationsControllerTests
{
	public class NotificationsControllerDeleteNotificationTests : NotificationsControllerPreparation
	{
		public NotificationsControllerDeleteNotificationTests() : base()
		{
		}
		[Fact]
		public async void DeleteNotification_ReturnsNoContentResult_WhenTheNotificationHasBeenRemoved()
		{
			//Arrange
			_mockUserService.Setup(Service => Service.CheckIfUserExists(It.IsAny<Guid>()))
				.ReturnsAsync(true)
				.Verifiable();
			_mockNotificationService.Setup(Service => Service.DeleteNotificationAsync(It.IsAny<Guid>()))
				.Verifiable();

			var controller = new NotificationsController(_loggerMock.Object, _mockNotificationService.Object, _mockUserService.Object);

			//Act
			var result = await controller.DeleteNotification(ConstIds.ExampleUserId, ConstIds.ExampleNotificationId);

			//Act
			var noContentResult = Assert.IsType<NoContentResult>(result);
			_mockUserService.Verify();
			_mockNotificationService.Verify();
		}
		[Theory]
		[InlineData(ConstIds.InvalidGuid, ConstIds.ExampleNotificationId)]
		[InlineData(ConstIds.ExampleUserId, ConstIds.InvalidGuid)]
		public async void DeleteNotification_ReturnsBadRequestObjectResult_WhenUserOrNotificationGuidIdIsInvalid(string testUserId, string testNotificationId)
		{
			//Arrange
			var controller = new NotificationsController(_loggerMock.Object, _mockNotificationService.Object, _mockUserService.Object);

			//Act
			var result = await controller.DeleteNotification(testUserId, testNotificationId);

			//Assert
			var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal($"User id: {testUserId} or notification id: {testNotificationId} is not valid guid.", badRequestObjectResult.Value);
		}
		[Fact]
		public async void DeleteNotificaton_ReturnsNotFoundObjectResult_WhenTheUserDoesntExist()
		{
			//Arrange
			_mockUserService.Setup(Service => Service.CheckIfUserExists(It.IsAny<Guid>()))
				.ReturnsAsync(false);

			var controller = new NotificationsController(_loggerMock.Object, _mockNotificationService.Object, _mockUserService.Object);

			//Act
			var result = await controller.DeleteNotification(ConstIds.ExampleUserId, ConstIds.ExampleNotificationId);

			//Assert
			var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result);
			Assert.Equal($"User: {ConstIds.ExampleUserId} not found.", notFoundObjectResult.Value);
			_mockUserService.Verify();
		}
		[Fact]
		public async void DeleteNotification_ReturnsInternalServerErrorResult_WhenExceptionThrownInService()
		{
			//Arrange
			_mockUserService.Setup(Service => Service.CheckIfUserExists(It.IsAny<Guid>()))
				.Throws(new ArgumentNullException(nameof(Guid)))
				.Verifiable();

			var controller = new NotificationsController(_loggerMock.Object, _mockNotificationService.Object, _mockUserService.Object);

			//Act
			var result = await controller.DeleteNotification(ConstIds.ExampleUserId, ConstIds.ExampleNotificationId);
			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockUserService.Verify();
		}
	}
}
