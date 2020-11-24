using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyFaceApi.Api.Controllers;
using MyFaceApi.Api.DataAccess.Entities;
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
			_mockUserRepo.Setup(repo => repo.CheckIfUserExists(It.IsAny<Guid>()))
				.Returns(true)
				.Verifiable();
			_mockNotificationRepo.Setup(repo => repo.GetNotification(It.IsAny<Guid>()))
				.Returns(notification)
				.Verifiable();

			var controller = new NotificationsController(_loggerMock.Object, _mockNotificationRepo.Object, _mapper, _mockUserRepo.Object);

			//Act
			var result = controller.GetNotification(ConstIds.ExampleUserId, ConstIds.ExampleNotificationId);

			//Assert
			var actionResult = Assert.IsType<OkObjectResult>(result.Result);
			var model = Assert.IsType<Notification>(actionResult.Value);
			Assert.Equal(notification, model);
			_mockUserRepo.Verify();
			_mockNotificationRepo.Verify();
		}
		[Theory]
		[InlineData(ConstIds.InvalidGuid, ConstIds.ExampleNotificationId)]
		[InlineData(ConstIds.ExampleUserId, ConstIds.InvalidGuid)]
		public void GetNotification_ReturnsBadRequestObjectResult_WhenUserOrNotificationIdIsInvalid(string testUserId, string testNotificationId)
		{
			//Arrange
			var controller = new NotificationsController(_loggerMock.Object, _mockNotificationRepo.Object, _mapper, _mockUserRepo.Object);

			//Act
			var result = controller.GetNotification(testUserId, testNotificationId);

			//Assert
			var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result.Result);
			Assert.Equal($"User id: {testUserId} or  notification id: {testNotificationId} is not valid Guid.", badRequestObjectResult.Value);
		}
		[Theory]
		[InlineData(true, null)]
		[InlineData(false, null)]
		public void GetNotification_ReturnsNotFoundObjectResult_WhenUserOrNotificationDoesntExist(bool doesTheUserExists, Notification testNotificationData)
		{
			//Arrange
			_mockUserRepo.Setup(repo => repo.CheckIfUserExists(It.IsAny<Guid>()))
				.Returns(doesTheUserExists)
				.Verifiable();
			_mockNotificationRepo.Setup(repo => repo.GetNotification(It.IsAny<Guid>()))
				.Returns(testNotificationData)
				.Verifiable();

			var controller = new NotificationsController(_loggerMock.Object, _mockNotificationRepo.Object, _mapper, _mockUserRepo.Object);

			//Act
			var result = controller.GetNotification(ConstIds.ExampleUserId, ConstIds.ExampleNotificationId);
			//Assert
			var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result.Result);

			if (doesTheUserExists)
			{
				_mockNotificationRepo.Verify();
				Assert.Equal($"Notification: {ConstIds.ExampleNotificationId} not found.", notFoundObjectResult.Value);
			}
			else
			{
				Assert.Equal($"User: {ConstIds.ExampleUserId} not found.", notFoundObjectResult.Value);
			}
			_mockUserRepo.Verify();
		}
		[Fact]
		public void GetNotification_ReturnsInternalServerErrorResult_WhenExceptionThrownInRepository()
		{
			//Arrange
			_mockUserRepo.Setup(repo => repo.CheckIfUserExists(It.IsAny<Guid>()))
				.Throws(new ArgumentNullException(nameof(Guid)))
				.Verifiable();

			var controller = new NotificationsController(_loggerMock.Object, _mockNotificationRepo.Object, _mapper, _mockUserRepo.Object);

			//Act
			var result = controller.GetNotification(ConstIds.ExampleUserId, ConstIds.ExampleNotificationId);
			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result.Result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockUserRepo.Verify();
		}
	}
}

