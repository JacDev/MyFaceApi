using System;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using MyFaceApi.Api.Controllers;
using Moq;
using Microsoft.AspNetCore.Http;

namespace MyFaceApi.Api.Tests.UnitTests.MessagesControllerTests
{
	public class MessagesControllerDeleteMessageTests : MessagesControllerPreparation
	{
		public MessagesControllerDeleteMessageTests() : base()
		{
		}
		[Fact]
		public async void DeleteMessage_ReturnsNoContentResult_WhenTheMessageHasBeenRemoved()
		{
			//Arrange
			_mockMessagesService.Setup(Service => Service.DeleteMessageAsync(It.IsAny<Guid>()))
				.Verifiable();

			var controller = new MessagesController(_loggerMock.Object, _mockMessagesService.Object, _mockUserService.Object);

			//Act
			var result = await controller.DeleteMessage(ConstIds.ExampleMessageId);

			//Act
			var noContentResult = Assert.IsType<NoContentResult>(result);
			_mockMessagesService.Verify();
		}
		[Fact]
		public async void DeleteMessage_ReturnsBadRequestObjectResult_WhenTheMessageGuidIdIsInvalid()
		{
			//Arrange
			var controller = new MessagesController(_loggerMock.Object, _mockMessagesService.Object, _mockUserService.Object);

			//Act
			var result = await controller.DeleteMessage(ConstIds.InvalidGuid);

			//Assert
			var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal($"{ConstIds.InvalidGuid} is not valid guid.", badRequestObjectResult.Value);
		}
		[Fact]
		public async void DeleteMessage_ReturnsInternalServerErrorResult_WhenExceptionThrownInService()
		{
			//Arrange
			_mockMessagesService.Setup(Service => Service.DeleteMessageAsync(It.IsAny<Guid>()))
				.Throws(new ArgumentNullException(nameof(Guid)))
				.Verifiable();

			var controller = new MessagesController(_loggerMock.Object, _mockMessagesService.Object, _mockUserService.Object);

			//Act
			var result = await controller.DeleteMessage(ConstIds.ExampleMessageId);

			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockMessagesService.Verify();
		}
	}
}
