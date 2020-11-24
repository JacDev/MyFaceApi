using Moq;
using MyFaceApi.Api.DataAccess.Entities;
using System;
using Xunit;
using AutoFixture;
using MyFaceApi.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

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
			var message = _fixture.Create<Message>();

			_mockMessagesRepo.Setup(repo => repo.GetMessage(It.IsAny<Guid>()))
					.Returns(message)
					.Verifiable();

			var controller = new MessagesController(_loggerMock.Object, _mockMessagesRepo.Object, _mockUserRepo.Object, _mapper);

			//Act
			var result = controller.GetMessage(ConstIds.ExampleUserId);

			//Assert
			var actionResult = Assert.IsType<OkObjectResult>(result.Result);
			var model = Assert.IsType<Message>(actionResult.Value);
			Assert.Equal(message, model);
			_mockMessagesRepo.Verify();
		}
		[Fact]
		public void GetMessage_ReturnsBadRequestObjectResult_WhenTheMessageIdIsInvalid()
		{
			//Arrange
			var controller = new MessagesController(_loggerMock.Object, _mockMessagesRepo.Object, _mockUserRepo.Object, _mapper);

			//Act
			var result = controller.GetMessage(ConstIds.InvalidGuid);

			//Assert
			var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result.Result);
			Assert.Equal($"{ConstIds.InvalidGuid} is not valid guid.", badRequestObjectResult.Value);
		}
		[Fact]
		public void GetMessage_ReturnsInternalServerErrorResult_WhenExceptionThrownInRepository()
		{
			//Arrange
			_mockMessagesRepo.Setup(repo => repo.GetMessage(It.IsAny<Guid>()))
				.Throws(new ArgumentNullException(nameof(Guid)))
				.Verifiable();

			var controller = new MessagesController(_loggerMock.Object, _mockMessagesRepo.Object, _mockUserRepo.Object, _mapper);

			//Act
			var result = controller.GetMessage(ConstIds.ExampleUserId);
			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result.Result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockMessagesRepo.Verify();
		}
	}
}
