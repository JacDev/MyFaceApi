using Moq;
using System;
using Xunit;
using AutoFixture;
using MyFaceApi.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using MyFaceApi.Api.Application.DtoModels.Message;

namespace MyFaceApi.Tests.UnitTests.MessagesControllerTests
{
	public class MessagesControllerGetMessageTests : MessagesControllerPreparation
	{
		public MessagesControllerGetMessageTests() : base()
		{
		}
		[Fact]
		public void GetMessage_ReturnsOkObjectResult_WithMessageData()
		{
			//Arrange
			var message = _fixture.Create<MessageDto>();

			_mockMessagesService.Setup(Service => Service.GetMessage(It.IsAny<Guid>()))
					.Returns(message)
					.Verifiable();

			var controller = new MessagesController(_loggerMock.Object, _mockMessagesService.Object, _mockUserService.Object);

			//Act
			var result = controller.GetMessage(ConstIds.ExampleUserId);

			//Assert
			var actionResult = Assert.IsType<OkObjectResult>(result.Result);
			var model = Assert.IsType<MessageDto>(actionResult.Value);
			Assert.Equal(message, model);
			_mockMessagesService.Verify();
		}
		[Fact]
		public void GetMessage_ReturnsBadRequestObjectResult_WhenTheMessageIdIsInvalid()
		{
			//Arrange
			var controller = new MessagesController(_loggerMock.Object, _mockMessagesService.Object, _mockUserService.Object);

			//Act
			var result = controller.GetMessage(ConstIds.InvalidGuid);

			//Assert
			var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result.Result);
			Assert.Equal($"{ConstIds.InvalidGuid} is not valid guid.", badRequestObjectResult.Value);
		}
		[Fact]
		public void GetMessage_ReturnsInternalServerErrorResult_WhenExceptionThrownInService()
		{
			//Arrange
			_mockMessagesService.Setup(Service => Service.GetMessage(It.IsAny<Guid>()))
				.Throws(new ArgumentNullException(nameof(Guid)))
				.Verifiable();

			var controller = new MessagesController(_loggerMock.Object, _mockMessagesService.Object, _mockUserService.Object);

			//Act
			var result = controller.GetMessage(ConstIds.ExampleUserId);
			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result.Result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockMessagesService.Verify();
		}
	}
}
