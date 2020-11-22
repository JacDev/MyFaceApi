using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyFaceApi.Controllers;
using MyFaceApi.DataAccess.Entities;
using MyFaceApi.Models.NotificationModels;
using System;
using Xunit;
using AutoFixture;

namespace MyFaceApi.Tests.UnitTests.NotificationsControllerTests
{
	public class NotificationsControllerAddNotificationTests : NotificationsControllerPreparation
	{
		protected readonly NotificationToAdd _notificationToAdd;
		public NotificationsControllerAddNotificationTests() : base()
		{
			_notificationToAdd = _fixture.Create<NotificationToAdd>();
		}
		[Fact]
		public async void AddNotification_ReturnsCreatedAtRouteResult_WithNotificationData()
		{
			//Arrange
			_mockUserRepo.Setup(repo => repo.CheckIfUserExists(It.IsAny<Guid>()))
				.Returns(true)
				.Verifiable();
			Notification notificationEntity = _mapper.Map<Notification>(_notificationToAdd);
			notificationEntity.Id = new Guid(ConstIds.ExampleNotificationId);

			_mockNotificationRepo.Setup(repo => repo.AddNotificationAsync(It.IsAny<Notification>()))
				.ReturnsAsync(notificationEntity)
				.Verifiable();

			var controller = new NotificationsController(_loggerMock.Object, _mockNotificationRepo.Object, _mapper, _mockUserRepo.Object);

			//Act
			var result = await controller.AddNotification(ConstIds.ExampleUserId, _notificationToAdd);

			//Assert
			var redirectToActionResult = Assert.IsType<CreatedAtRouteResult>(result.Result);
			Assert.Equal(ConstIds.ExampleUserId, redirectToActionResult.RouteValues["userId"].ToString());
			Assert.Equal(ConstIds.ExampleNotificationId, redirectToActionResult.RouteValues["notificationId"].ToString());
			Assert.Equal("GetNotification", redirectToActionResult.RouteName);
			_mockUserRepo.Verify();
			_mockNotificationRepo.Verify();
		}
		[Fact]
		public async void AddNotification_ReturnsBadRequestObjectResult_WhenTheUserIdIsInvalid()
		{
			//Arrange
			var controller = new NotificationsController(_loggerMock.Object, _mockNotificationRepo.Object, _mapper, _mockUserRepo.Object);

			//Act
			var result = await controller.AddNotification(ConstIds.InvalidGuid, _notificationToAdd);

			//Assert
			var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result.Result);
			Assert.Equal($"{ConstIds.InvalidGuid} is not valid Guid.", badRequestObjectResult.Value);
		}
		[Fact]
		public async void AddNotification_ReturnsNotFoundObjectResult_WhenTheUserDoesntExist()
		{
			//Arrange
			_mockUserRepo.Setup(repo => repo.CheckIfUserExists(It.IsAny<Guid>()))
				.Returns(false)
				.Verifiable();

			var controller = new NotificationsController(_loggerMock.Object, _mockNotificationRepo.Object, _mapper, _mockUserRepo.Object);

			//Act
			var result = await controller.AddNotification(ConstIds.ExampleUserId, _notificationToAdd);

			//Assert
			var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result.Result);
			Assert.Equal($"User: {ConstIds.ExampleUserId} not found.", notFoundObjectResult.Value);
			_mockUserRepo.Verify();
		}
		[Fact]
		public async void AddNotification_ReturnsInternalServerErrorResult_WhenExceptionThrownInRepository()
		{
			//Arrange
			_mockUserRepo.Setup(repo => repo.CheckIfUserExists(It.IsAny<Guid>()))
				.Throws(new ArgumentNullException(nameof(Guid)))
				.Verifiable();

			var controller = new NotificationsController(_loggerMock.Object, _mockNotificationRepo.Object, _mapper, _mockUserRepo.Object);

			//Act
			var result = await controller.AddNotification(ConstIds.ExampleUserId, _notificationToAdd);
			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result.Result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockUserRepo.Verify();
		}
	}
}
