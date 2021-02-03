using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyFaceApi.Api.Application.DtoModels.Notification;
using MyFaceApi.Api.Controllers;
using Pagination.DtoModels;
using Pagination.Helpers;
using System;
using Xunit;

namespace MyFaceApi.Api.Tests.UnitTests.NotificationsControllerTests
{
	public class NotificationsControllerGetNotificationsTests : NotificationsControllerPreparation
	{
		protected readonly PaginationParams _paginationsParams;

		public NotificationsControllerGetNotificationsTests() : base()
		{
			_paginationsParams = new PaginationParams()
			{
				PageNumber = 0,
				PageSize = 10,
				Skip = 0
			};
		}
		[Fact]
		public async void GetNotifications_ReturnsOkObjectResult_WithAListOfNotificationsData()
		{
			//Arrange
			var notifications = GetTestNotificationData();
			var pagedList = PagedList<NotificationDto>.Create(notifications, _paginationsParams.PageNumber, _paginationsParams.PageSize, _paginationsParams.Skip);
			_mockUserService.Setup(Service => Service.CheckIfUserExists(It.IsAny<Guid>()))
				.ReturnsAsync(true)
				.Verifiable();
			_mockNotificationService.Setup(Service => Service.GetUserNotifications(It.IsAny<Guid>(), _paginationsParams, null, 0, null))
				.Returns(pagedList)
				.Verifiable();

			var mockUrlHelper = new Mock<IUrlHelper>();
			mockUrlHelper
				.Setup(m => m.Link("GetNotifications", It.IsAny<object>()))
				.Returns("some url")
				.Verifiable();

			var controller = new NotificationsController(_loggerMock.Object, _mockNotificationService.Object, _mockUserService.Object)
			{
				Url = mockUrlHelper.Object
			};

			//Act
			var result = await controller.GetNotifications(ConstIds.ExampleUserId, _paginationsParams);

			//Assert
			var actionResult = Assert.IsType<OkObjectResult>(result.Result);
			var model = Assert.IsType<CollectionWithPaginationData<NotificationDto>>(actionResult.Value);

			Assert.Equal(notifications.Count, model.Collection.Count);
			_mockUserService.Verify();
			_mockNotificationService.Verify();
		}
		[Fact]
		public async void GetNotifications_ReturnsBadRequestObjectResult_WhenTheUserIsInvalid()
		{
			//Arrange
			var controller = new NotificationsController(_loggerMock.Object, _mockNotificationService.Object, _mockUserService.Object);

			//Act
			var result = await controller.GetNotifications(ConstIds.InvalidGuid, _paginationsParams);

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
			var result = await controller.GetNotifications(ConstIds.ExampleUserId, _paginationsParams);
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
			var result = await controller.GetNotifications(ConstIds.ExampleUserId, _paginationsParams);
			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result.Result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockUserService.Verify();
		}
	}
}
