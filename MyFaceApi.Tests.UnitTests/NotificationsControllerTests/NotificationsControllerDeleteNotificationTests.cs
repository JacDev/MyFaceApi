using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyFaceApi.Api.Controllers;
using MyFaceApi.Api.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
			var notificationToRemove = GetTestNotificationData().ElementAt(0);
			_mockUserRepo.Setup(repo => repo.CheckIfUserExists(It.IsAny<Guid>()))
				.ReturnsAsync(true)
				.Verifiable();
			_mockNotificationRepo.Setup(repo => repo.GetNotification(It.IsAny<Guid>()))
				.Returns(notificationToRemove)
				.Verifiable();
			_mockNotificationRepo.Setup(repo => repo.DeleteNotificationAsync(It.IsAny<Notification>()))
				.Verifiable();

			var controller = new NotificationsController(_loggerMock.Object, _mockNotificationRepo.Object, _mapper, _mockUserRepo.Object);

			//Act
			var result = await controller.DeleteNotification(ConstIds.ExampleUserId, ConstIds.ExampleNotificationId);

			//Act
			var noContentResult = Assert.IsType<NoContentResult>(result);
			_mockUserRepo.Verify();
			_mockNotificationRepo.Verify();
		}

		[Theory]
		[InlineData(ConstIds.InvalidGuid, ConstIds.ExampleNotificationId)]
		[InlineData(ConstIds.ExampleUserId, ConstIds.InvalidGuid)]
		public async void DeleteNotification_ReturnsBadRequestObjectResult_WhenUserOrNotificationGuidIdIsInvalid(string testUserId, string testNotificationId)
		{
			//Arrange
			var controller = new NotificationsController(_loggerMock.Object, _mockNotificationRepo.Object, _mapper, _mockUserRepo.Object);

			//Act
			var result = await controller.DeleteNotification(testUserId, testNotificationId);

			//Assert
			var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal($"User id: {testUserId} or notification id: {testNotificationId} is not valid Guid.", badRequestObjectResult.Value);
		}
		[Theory]
		[InlineData(true, null)]
		[InlineData(false, null)]
		public async void DeleteNotificaton_ReturnsNotFoundObjectResult_WhenTheUserOrNotificationDoesntExist(bool doesTheUserExists, Notification testNotificationData)
		{
			//Arrange
			_mockUserRepo.Setup(repo => repo.CheckIfUserExists(It.IsAny<Guid>()))
				.ReturnsAsync(doesTheUserExists);
			_mockNotificationRepo.Setup(repo => repo.GetNotification(It.IsAny<Guid>()))
				.Returns(testNotificationData)
				.Verifiable();

			var controller = new NotificationsController(_loggerMock.Object, _mockNotificationRepo.Object, _mapper, _mockUserRepo.Object);

			//Act
			var result = await controller.DeleteNotification(ConstIds.ExampleUserId, ConstIds.ExampleNotificationId);

			//Assert
			var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result);

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
		public async void DeleteNotification_ReturnsInternalServerErrorResult_WhenExceptionThrownInRepository()
		{
			//Arrange
			_mockUserRepo.Setup(repo => repo.CheckIfUserExists(It.IsAny<Guid>()))
				.Throws(new ArgumentNullException(nameof(Guid)))
				.Verifiable();

			var controller = new NotificationsController(_loggerMock.Object, _mockNotificationRepo.Object, _mapper, _mockUserRepo.Object);

			//Act
			var result = await controller.DeleteNotification(ConstIds.ExampleUserId, ConstIds.ExampleNotificationId);
			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockUserRepo.Verify();
		}
	}
}
