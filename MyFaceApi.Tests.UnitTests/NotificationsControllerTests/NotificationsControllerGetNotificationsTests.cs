using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyFaceApi.Controllers;
using MyFaceApi.DataAccess.Entities;
using System;
using System.Collections.Generic;
using Xunit;

namespace MyFaceApi.Tests.UnitTests.NotificationsControllerTests
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

			_mockUserRepo.Setup(repo => repo.CheckIfUserExists(It.IsAny<Guid>()))
				.Returns(true)
				.Verifiable();
			_mockNotificationRepo.Setup(repo => repo.GetUserNotifications(It.IsAny<Guid>()))
				.Returns(notifications)
				.Verifiable();

			var controller = new NotificationsController(_loggerMock.Object, _mockNotificationRepo.Object, _mapper, _mockUserRepo.Object);

			//Act
			var result = controller.GetNotifications(ConstIds.ExampleUserId);

			//Assert
			var actionResult = Assert.IsType<OkObjectResult>(result.Result);
			var model = Assert.IsType<List<Notification>>(actionResult.Value);

			Assert.Equal(notifications.Count, model.Count);
			_mockUserRepo.Verify();
			_mockNotificationRepo.Verify();
		}
		[Fact]
		public void GetNotifications_ReturnsBadRequestObjectResult_WhenTheUserIsInvalid()
		{
			//Arrange
			var controller = new NotificationsController(_loggerMock.Object, _mockNotificationRepo.Object, _mapper, _mockUserRepo.Object);

			//Act
			var result = controller.GetNotifications(ConstIds.InvalidGuid);

			//Assert
			var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result.Result);
			Assert.Equal($"{ConstIds.InvalidGuid} is not valid Guid.", badRequestObjectResult.Value);
		}
		[Theory]
		[InlineData(true, null)]
		[InlineData(false, null)]
		public void GetNotifications_ReturnsNotFoundObjectResult_WhenUserOrNotificationDoesntExist(bool doesTheUserExists, List<Notification> testNotificationData)
		{
			//Arrange
			_mockUserRepo.Setup(repo => repo.CheckIfUserExists(It.IsAny<Guid>()))
				.Returns(doesTheUserExists)
				.Verifiable();
			_mockNotificationRepo.Setup(repo => repo.GetUserNotifications(It.IsAny<Guid>()))
				.Returns(testNotificationData)
				.Verifiable();

			var controller = new NotificationsController(_loggerMock.Object, _mockNotificationRepo.Object, _mapper, _mockUserRepo.Object);

			//Act
			var result = controller.GetNotifications(ConstIds.ExampleUserId);
			//Assert
			var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result.Result);

			if (doesTheUserExists)
			{
				_mockNotificationRepo.Verify();
				Assert.Equal($"The user: {ConstIds.ExampleUserId} notificatios not found.", notFoundObjectResult.Value);
			}
			else
			{
				Assert.Equal($"User: {ConstIds.ExampleUserId} not found.", notFoundObjectResult.Value);
			}
			_mockUserRepo.Verify();
		}
		[Fact]
		public void GetNotifications_ReturnsInternalServerErrorResult_WhenExceptionThrownInRepository()
		{
			//Arrange
			_mockUserRepo.Setup(repo => repo.CheckIfUserExists(It.IsAny<Guid>()))
				.Throws(new ArgumentNullException(nameof(Guid)))
				.Verifiable();

			var controller = new NotificationsController(_loggerMock.Object, _mockNotificationRepo.Object, _mapper, _mockUserRepo.Object);

			//Act
			var result = controller.GetNotifications(ConstIds.ExampleUserId);
			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result.Result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockUserRepo.Verify();
		}
	}
}
