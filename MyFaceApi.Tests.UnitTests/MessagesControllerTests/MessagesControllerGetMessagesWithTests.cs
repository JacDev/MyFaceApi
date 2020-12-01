using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using AutoFixture;
using MyFaceApi.Api.DataAccess.Entities;
using Moq;
using MyFaceApi.Api.Controllers;
using MyFaceApi.Api.Repository.Helpers;
using Microsoft.AspNetCore.Mvc;
using MyFaceApi.Api.Extensions;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace MyFaceApi.Tests.UnitTests.MessagesControllerTests
{
	public class MessagesControllerGetMessagesWithTests : MessagesControllerPreparation
	{
		private readonly PaginationParams _paginationParams;
		public MessagesControllerGetMessagesWithTests() : base()
		{
			_paginationParams = new PaginationParams { PageNumber = 1, PageSize = 10, Skip = 0 };
		}
		[Fact]
		public void GetMessagesWith_ReturnsOkObjectResult_WithAPagedListOfMessagesData()
		{
			//Arrange
			var messages = new List<Message>();
			_fixture.AddManyTo(messages, 15);

			var pagedList = PagedList<Message>.Create(messages,
				_paginationParams.PageNumber,
				_paginationParams.PageSize,
				_paginationParams.Skip);
			_mockUserRepo.Setup(repo => repo.CheckIfUserExists(It.IsAny<Guid>()))
				.ReturnsAsync(true)
				.Verifiable();
			_mockMessagesRepo.Setup(repo => repo.GetUserMessagesWith(It.IsAny<Guid>(), It.IsAny<Guid>(), _paginationParams))
				.Returns(pagedList)
				.Verifiable();

			var mockUrlHelper = new Mock<IUrlHelper>();
			mockUrlHelper
				.Setup(m => m.Link("GetMessages", It.IsAny<object>()))
				.Returns("some url")
				.Verifiable();

			var httpContextMock = new Mock<HttpContext>();
			httpContextMock.Setup(r => r.Response.Headers.Add(It.IsAny<string>(), It.IsAny<StringValues>()))
			.Verifiable();

			var controllerContextMock = new Mock<ControllerContext>();
			controllerContextMock.Object.HttpContext = httpContextMock.Object;

			var controller = new MessagesController(_loggerMock.Object, _mockMessagesRepo.Object, _mockUserRepo.Object, _mapper)
			{
				ControllerContext = controllerContextMock.Object,
				Url = mockUrlHelper.Object
			};
			//Act
			var result = controller.GetMessagesWith(ConstIds.ExampleUserId, ConstIds.ExampleFromWhoId, _paginationParams);

			//Assert
			var actionResult = Assert.IsType<OkObjectResult>(result.Result);
			var model = Assert.IsType<PagedList<Message>>(actionResult.Value);

			Assert.Equal(_paginationParams.PageSize, model.Count);
			_mockUserRepo.Verify();
			_mockMessagesRepo.Verify();
			httpContextMock.Verify();
			mockUrlHelper.Verify();
		}
		[Theory]
		[InlineData(ConstIds.InvalidGuid, ConstIds.ExampleNotificationId)]
		[InlineData(ConstIds.ExampleUserId, ConstIds.InvalidGuid)]
		public void GetMessages_ReturnsBadRequestObjectResult_WhenTheUserOrWithWhoIdIsInvalid(string testUserId, string testFriendId)
		{
			//Arrange
			var controller = new MessagesController(_loggerMock.Object, _mockMessagesRepo.Object, _mockUserRepo.Object, _mapper);

			//Act
			var result = controller.GetMessagesWith(testUserId, testFriendId, _paginationParams);

			//Assert
			var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result.Result);
			Assert.Equal($"{testUserId} or user: {testFriendId} is not valid guid.", badRequestObjectResult.Value);
		}
		[Theory]
		[InlineData(true, false)]
		[InlineData(false, true)]
		public void GetMessages_ReturnsNotFoundObjectResult_WhenTheUserOrWithWhoDoesntExist(bool doesTheUserExist, bool doesTheFriendExist)
		{
			//Arrange
			_mockUserRepo.Setup(repo => repo.CheckIfUserExists(It.IsAny<Guid>()))
				.ReturnsAsync(doesTheUserExist)
				.Verifiable();
			if (doesTheUserExist)
			{
				_mockUserRepo.Setup(repo => repo.CheckIfUserExists(It.IsNotIn<Guid>(new Guid(ConstIds.ExampleUserId))))
					.ReturnsAsync(doesTheFriendExist)
					.Verifiable();
			}
			var controller = new MessagesController(_loggerMock.Object, _mockMessagesRepo.Object, _mockUserRepo.Object, _mapper);

			//Act
			var result = controller.GetMessagesWith(ConstIds.ExampleUserId, ConstIds.ExampleFromWhoId, _paginationParams);
			//Assert
			var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result.Result);
			Assert.Equal($"User: {ConstIds.ExampleUserId} or user: {ConstIds.ExampleFromWhoId} doesnt exist.", notFoundObjectResult.Value);
			_mockUserRepo.Verify();
		}
		[Fact]
		public void GetMessages_ReturnsInternalServerErrorResult_WhenExceptionThrownInRepository()
		{
			//Arrange
			_mockUserRepo.Setup(repo => repo.CheckIfUserExists(new Guid(ConstIds.ExampleUserId)))
				.Throws(new ArgumentNullException(nameof(Guid)))
				.Verifiable();

			var controller = new MessagesController(_loggerMock.Object, _mockMessagesRepo.Object, _mockUserRepo.Object, _mapper);

			//Act
			var result = controller.GetMessagesWith(ConstIds.ExampleUserId, ConstIds.ExampleFromWhoId, _paginationParams);

			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result.Result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockUserRepo.Verify();
		}
	}
}
