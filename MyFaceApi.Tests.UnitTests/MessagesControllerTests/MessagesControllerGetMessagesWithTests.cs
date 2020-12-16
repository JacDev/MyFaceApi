using System;
using System.Collections.Generic;
using Xunit;
using AutoFixture;
using Moq;
using MyFaceApi.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Pagination.Helpers;
using MyFaceApi.Api.Application.DtoModels.Message;

namespace MyFaceApi.Api.Tests.UnitTests.MessagesControllerTests
{
	public class MessagesControllerGetMessagesWithTests : MessagesControllerPreparation
	{
		private readonly PaginationParams _paginationParams;
		public MessagesControllerGetMessagesWithTests() : base()
		{
			_paginationParams = new PaginationParams { PageNumber = 1, PageSize = 10, Skip = 0 };
		}
		[Fact]
		public async void GetMessagesWith_ReturnsOkObjectResult_WithAPagedListOfMessagesData()
		{
			//Arrange
			var messages = new List<MessageDto>();
			_fixture.AddManyTo(messages, 15);

			var pagedList = PagedList<MessageDto>.Create(messages,
				_paginationParams.PageNumber,
				_paginationParams.PageSize,
				_paginationParams.Skip);
			_mockUserService.Setup(Service => Service.CheckIfUserExists(It.IsAny<Guid>()))
				.ReturnsAsync(true)
				.Verifiable();
			_mockMessagesService.Setup(Service => Service.GetUserMessagesWith(It.IsAny<Guid>(), It.IsAny<Guid>(), _paginationParams))
				.Returns(pagedList)
				.Verifiable();


			var controller = new MessagesController(_loggerMock.Object, _mockMessagesService.Object, _mockUserService.Object);

			//Act
			var result = await controller.GetMessagesWith(ConstIds.ExampleUserId, ConstIds.ExampleFromWhoId, _paginationParams);

			//Assert
			var actionResult = Assert.IsType<OkObjectResult>(result.Result);
			var model = Assert.IsType<PagedList<MessageDto>>(actionResult.Value);

			Assert.Equal(_paginationParams.PageSize, model.Count);
			_mockUserService.Verify();
			_mockMessagesService.Verify();
		}
		[Theory]
		[InlineData(ConstIds.InvalidGuid, ConstIds.ExampleNotificationId)]
		[InlineData(ConstIds.ExampleUserId, ConstIds.InvalidGuid)]
		public async void GetMessages_ReturnsBadRequestObjectResult_WhenTheUserOrWithWhoIdIsInvalid(string testUserId, string testFriendId)
		{
			//Arrange
			var controller = new MessagesController(_loggerMock.Object, _mockMessagesService.Object, _mockUserService.Object);

			//Act
			var result = await controller.GetMessagesWith(testUserId, testFriendId, _paginationParams);

			//Assert
			var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result.Result);
			Assert.Equal($"{testUserId} or user: {testFriendId} is not valid guid.", badRequestObjectResult.Value);
		}
		[Theory]
		[InlineData(true, false)]
		[InlineData(false, true)]
		public async void GetMessages_ReturnsNotFoundObjectResult_WhenTheUserOrWithWhoDoesntExist(bool doesTheUserExist, bool doesTheFriendExist)
		{
			//Arrange
			_mockUserService.Setup(Service => Service.CheckIfUserExists(It.IsAny<Guid>()))
				.ReturnsAsync(doesTheUserExist)
				.Verifiable();
			if (doesTheUserExist)
			{
				_mockUserService.Setup(Service => Service.CheckIfUserExists(It.IsNotIn<Guid>(new Guid(ConstIds.ExampleUserId))))
					.ReturnsAsync(doesTheFriendExist)
					.Verifiable();
			}
			var controller = new MessagesController(_loggerMock.Object, _mockMessagesService.Object, _mockUserService.Object);

			//Act
			var result = await controller.GetMessagesWith(ConstIds.ExampleUserId, ConstIds.ExampleFromWhoId, _paginationParams);
			//Assert
			var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result.Result);
			Assert.Equal($"User: {ConstIds.ExampleUserId} or user: {ConstIds.ExampleFromWhoId} doesnt exist.", notFoundObjectResult.Value);
			_mockUserService.Verify();
		}
		[Fact]
		public async void GetMessages_ReturnsInternalServerErrorResult_WhenExceptionThrownInService()
		{
			//Arrange
			_mockUserService.Setup(Service => Service.CheckIfUserExists(new Guid(ConstIds.ExampleUserId)))
				.Throws(new ArgumentNullException(nameof(Guid)))
				.Verifiable();

			var controller = new MessagesController(_loggerMock.Object, _mockMessagesService.Object, _mockUserService.Object);

			//Act
			var result = await controller.GetMessagesWith(ConstIds.ExampleUserId, ConstIds.ExampleFromWhoId, _paginationParams);

			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result.Result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockUserService.Verify();
		}
	}
}
