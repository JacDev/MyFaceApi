using MyFaceApi.DataAccess.Entities;
using System;
using Xunit;
using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using MyFaceApi.Api.Controllers;
using Moq;
using Microsoft.AspNetCore.Http;

namespace MyFaceApi.Tests.UnitTests.MessagesControllerTests
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
			var messageToRemobe = _fixture.Create<Message>();
			_mockMessagesRepo.Setup(repo => repo.GetMessage(It.IsAny<Guid>()))
				.Returns(messageToRemobe)
				.Verifiable();
			_mockMessagesRepo.Setup(repo => repo.DeleteMessageAsync(It.IsAny<Message>()))
				.Verifiable();

			var controller = new MessagesController(_loggerMock.Object, _mockMessagesRepo.Object, _mockUserRepo.Object, _mapper);

			//Act
			var result = await controller.DeleteMessage(ConstIds.ExampleMessageId);

			//Act
			var noContentResult = Assert.IsType<NoContentResult>(result);
			_mockMessagesRepo.Verify();
		}
		[Fact]
		public async void DeleteMessage_ReturnsBadRequestObjectResult_WhenTheMessageGuidIdIsInvalid()
		{
			//Arrange
			var controller = new MessagesController(_loggerMock.Object, _mockMessagesRepo.Object, _mockUserRepo.Object, _mapper);

			//Act
			var result = await controller.DeleteMessage(ConstIds.InvalidGuid);

			//Assert
			var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal($"{ConstIds.InvalidGuid} is not valid guid.", badRequestObjectResult.Value);
		}
		[Fact]
		public async void DeleteMessage_ReturnsNotFoundResult_WhenTheMessageDoesntExist()
		{
			//Arrange
			_mockMessagesRepo.Setup(repo => repo.GetMessage(It.IsAny<Guid>()))
				.Returns((Message)null)
				.Verifiable();

			var controller = new MessagesController(_loggerMock.Object, _mockMessagesRepo.Object, _mockUserRepo.Object, _mapper);

			//Act
			var result = await controller.DeleteMessage(ConstIds.ExampleUserId);

			//Assert
			var notFoundResult = Assert.IsType<NotFoundResult>(result);
			_mockMessagesRepo.Verify();
		}
		[Fact]
		public async void DeleteMessage_ReturnsInternalServerErrorResult_WhenExceptionThrownInRepository()
		{
			//Arrange
			_mockMessagesRepo.Setup(repo => repo.GetMessage(It.IsAny<Guid>()))
				.Throws(new ArgumentNullException(nameof(Guid)))
				.Verifiable();

			var controller = new MessagesController(_loggerMock.Object, _mockMessagesRepo.Object, _mockUserRepo.Object, _mapper);

			//Act
			var result = await controller.DeleteMessage(ConstIds.ExampleMessageId);

			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockMessagesRepo.Verify();
		}
	}
}
