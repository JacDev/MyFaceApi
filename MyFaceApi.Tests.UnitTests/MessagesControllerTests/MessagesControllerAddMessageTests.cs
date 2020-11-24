using MyFaceApi.Api.Models.MessageModels;
using System;
using System.Collections.Generic;
using System.Text;
using AutoFixture;
using Xunit;
using MyFaceApi.Api.DataAccess.Entities;
using Moq;
using MyFaceApi.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace MyFaceApi.Tests.UnitTests.MessagesControllerTests
{
	public class MessagesControllerAddMessageTests : MessagesControllerPreparation
	{
		private readonly MessageToAdd _messageToAdd;
		public MessagesControllerAddMessageTests() : base()
		{
			_messageToAdd = _fixture.Create<MessageToAdd>();
		}
		[Fact]
		public async void AddMessage_ReturnsCreatedAtRouteResult_WithMessageData()
		{
			//Arrange
			_mockUserRepo.Setup(repo => repo.CheckIfUserExists(It.IsAny<Guid>()))
				.Returns(true)
				.Verifiable();
			Message messageEntity = _mapper.Map<Message>(_messageToAdd);
			messageEntity.Id = new Guid(ConstIds.ExampleMessageId);

			_mockMessagesRepo.Setup(repo => repo.AddMessageAsync(It.IsAny<Message>()))
				.ReturnsAsync(messageEntity)
				.Verifiable();

			var controller = new MessagesController(_loggerMock.Object, _mockMessagesRepo.Object, _mockUserRepo.Object, _mapper);

			//Act
			var result = await controller.AddMessage(ConstIds.ExampleUserId, _messageToAdd);

			//Assert
			var redirectToActionResult = Assert.IsType<CreatedAtRouteResult>(result.Result);
			Assert.Equal(ConstIds.ExampleUserId, redirectToActionResult.RouteValues["userId"].ToString());
			Assert.Equal(ConstIds.ExampleMessageId, redirectToActionResult.RouteValues["messageId"].ToString());
			Assert.Equal("GetMessage", redirectToActionResult.RouteName);
			Assert.IsType<Message>(redirectToActionResult.Value);

			_mockUserRepo.Verify();
			_mockMessagesRepo.Verify();
		}
		[Fact]
		public async void AddMessage_ReturnsBadRequestObjectResult_WhenTheUserIdIsInvalid()
		{
			//Arrange
			var controller = new MessagesController(_loggerMock.Object, _mockMessagesRepo.Object, _mockUserRepo.Object, _mapper);

			//Act
			var result = await controller.AddMessage(ConstIds.InvalidGuid, _messageToAdd);

			//Assert
			var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result.Result);
			Assert.Equal($"{ConstIds.InvalidGuid} is not valid guid.", badRequestObjectResult.Value);
		}
		[Fact]
		public async void AddMessage_ReturnsNotFoundObjectResult_WhenTheUserDoesntExist()
		{
			//Arrange
			_mockUserRepo.Setup(repo => repo.CheckIfUserExists(It.IsAny<Guid>()))
				.Returns(false)
				.Verifiable();

			var controller = new MessagesController(_loggerMock.Object, _mockMessagesRepo.Object, _mockUserRepo.Object, _mapper);

			//Act
			var result = await controller.AddMessage(ConstIds.ExampleUserId, _messageToAdd);

			//Assert
			var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result.Result);
			Assert.Equal($"User: {ConstIds.ExampleUserId} not found.", notFoundObjectResult.Value);
			_mockUserRepo.Verify();
		}
		[Fact]
		public async void AddPost_ReturnsInternalServerErrorResult_WhenExceptionThrownInRepository()
		{
			//Arrange
			_mockUserRepo.Setup(repo => repo.CheckIfUserExists(It.IsAny<Guid>()))
				.Throws(new ArgumentNullException(nameof(Guid)))
				.Verifiable();

			var controller = new MessagesController(_loggerMock.Object, _mockMessagesRepo.Object, _mockUserRepo.Object, _mapper);

			//Act
			var result = await controller.AddMessage(ConstIds.ExampleUserId, _messageToAdd);
			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result.Result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockUserRepo.Verify();
		}
	}
}
