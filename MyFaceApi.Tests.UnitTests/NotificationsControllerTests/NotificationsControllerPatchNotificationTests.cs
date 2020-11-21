using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Moq;
using MyFaceApi.Controllers;
using MyFaceApi.DataAccess.Entities;
using System;
using System.Linq;
using Xunit;

namespace MyFaceApi.Tests.UnitTests.NotificationsControllerTests
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
			var notification = GetTestNotificationData().ElementAt(0);

			_mockUserRepo.Setup(repo => repo.CheckIfUserExists(It.IsAny<Guid>()))
				.Returns(true)
				.Verifiable();
			_mockNotificationRepo.Setup(repo => repo.GetNotification(It.IsAny<Guid>()))
				.Returns(notification)
				.Verifiable();
			_mockNotificationRepo.Setup(repo => repo.UpdateNotificationAsync(It.IsAny<Notification>()))
				.Verifiable();

			var controller = new NotificationsController(_loggerMock.Object, _mockNotificationRepo.Object, _mapper, _mockUserRepo.Object);

			var objectValidator = new Mock<IObjectModelValidator>();
			objectValidator.Setup(o => o.Validate(It.IsAny<ActionContext>(),
											  It.IsAny<ValidationStateDictionary>(),
											  It.IsAny<string>(),
											  It.IsAny<Object>()));
			controller.ObjectValidator = objectValidator.Object;

			//Act
			var result = await controller.PartiallyUpdateNotification(ConstIds.ExampleUserId, ConstIds.ExampleNotificationId, GetJsonPatchDocument());

			//Assert
			var noContentResult = Assert.IsType<NoContentResult>(result);
			Assert.Equal(true.ToString(), notification.HasSeen.ToString());
			_mockUserRepo.Verify();
			_mockNotificationRepo.Verify();
		}
		[Theory]
		[InlineData(ConstIds.InvalidGuid, ConstIds.ExampleNotificationId)]
		[InlineData(ConstIds.ExampleUserId, ConstIds.InvalidGuid)]
		public async void PartiallyUpdateNotification_ReturnsBadRequestObjectResult_WhenUserOrNotificationIdIsInvalid(string testUserId, string testNotificationId)
		{
			//Arrange
			var controller = new NotificationsController(_loggerMock.Object, _mockNotificationRepo.Object, _mapper, _mockUserRepo.Object);

			//Act
			var result = await controller.PartiallyUpdateNotification(testUserId, testNotificationId, GetJsonPatchDocument());

			//Assert
			var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal($"User id: {testUserId} or notification id: {testNotificationId} is not valid Guid.", badRequestObjectResult.Value);
		}
		[Theory]
		[InlineData(true, null)]
		[InlineData(false, null)]
		public async void PartiallyUpdateNotification_NotFoundObjectRequest_WhenTheUserOrNotificationIsNotInTheDatabase(bool doesTheUserExists, Notification testNotificationData)
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
			var result = await controller.PartiallyUpdateNotification(ConstIds.ExampleUserId, ConstIds.ExampleNotificationId, GetJsonPatchDocument());

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
		public async void PartiallyUpdateNotification_ReturnsInternalServerErrorResult_WhenExceptionThrownInRepository()
		{
			//Arrange
			_mockUserRepo.Setup(repo => repo.CheckIfUserExists(It.IsAny<Guid>()))
				.Throws(new ArgumentNullException(nameof(Guid)))
				.Verifiable();

			var controller = new NotificationsController(_loggerMock.Object, _mockNotificationRepo.Object, _mapper, _mockUserRepo.Object);

			//Act
			var result = await controller.PartiallyUpdateNotification(ConstIds.ExampleUserId, ConstIds.ExampleNotificationId, GetJsonPatchDocument());

			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockUserRepo.Verify();
		}
	}
}
