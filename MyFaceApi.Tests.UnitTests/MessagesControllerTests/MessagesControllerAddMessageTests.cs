using System;
using AutoFixture;
using Xunit;
using Moq;
using MyFaceApi.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using MyFaceApi.Api.Application.DtoModels.Message;

namespace MyFaceApi.Api.Tests.UnitTests.MessagesControllerTests
{
	public class MessagesControllerAddMessageTests : MessagesControllerPreparation
	{
		private readonly MessageToAddDto _messageToAdd;
		public MessagesControllerAddMessageTests() : base()
		{
			_messageToAdd = _fixture.Create<MessageToAddDto>();
		}
		[Fact]
		public async void AddMessage_ReturnsCreatedAtRouteResult_WithMessageData()
		{
			//Arrange
			_mockUserService.Setup(Service => Service.CheckIfUserExists(It.IsAny<Guid>()))
				.ReturnsAsync(true)
				.Verifiable();
			MessageDto messageEntity = _mapper.Map<MessageDto>(_messageToAdd);
			messageEntity.Id = new Guid(ConstIds.ExampleMessageId);

			_mockMessagesService.Setup(Service => Service.AddMessageAsync(It.IsAny<Guid>(), It.IsAny<MessageToAddDto>()))
				.ReturnsAsync(messageEntity)
				.Verifiable();

			var controller = new MessagesController(_loggerMock.Object, _mockMessagesService.Object, _mockUserService.Object);

			//Act
			var result = await controller.AddMessage(ConstIds.ExampleUserId, _messageToAdd);

			//Assert
			var redirectToActionResult = Assert.IsType<CreatedAtRouteResult>(result.Result);
			Assert.Equal(ConstIds.ExampleUserId, redirectToActionResult.RouteValues["userId"].ToString());
			Assert.Equal(ConstIds.ExampleMessageId, redirectToActionResult.RouteValues["messageId"].ToString());
			Assert.Equal("GetMessage", redirectToActionResult.RouteName);
			Assert.IsType<MessageDto>(redirectToActionResult.Value);

			_mockUserService.Verify();
			_mockMessagesService.Verify();
		}
		[Fact]
		public async void AddMessage_ReturnsBadRequestObjectResult_WhenTheUserIdIsInvalid()
		{
			//Arrange
			var controller = new MessagesController(_loggerMock.Object, _mockMessagesService.Object, _mockUserService.Object);

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
			_mockUserService.Setup(Service => Service.CheckIfUserExists(It.IsAny<Guid>()))
				.ReturnsAsync(false)
				.Verifiable();

			var controller = new MessagesController(_loggerMock.Object, _mockMessagesService.Object, _mockUserService.Object);

			//Act
			var result = await controller.AddMessage(ConstIds.ExampleUserId, _messageToAdd);

			//Assert
			var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result.Result);
			Assert.Equal($"User: {ConstIds.ExampleUserId} not found.", notFoundObjectResult.Value);
			_mockUserService.Verify();
		}
		[Fact]
		public async void AddPost_ReturnsInternalServerErrorResult_WhenExceptionThrownInService()
		{
			//Arrange
			_mockUserService.Setup(Service => Service.CheckIfUserExists(It.IsAny<Guid>()))
				.Throws(new ArgumentNullException(nameof(Guid)))
				.Verifiable();

			var controller = new MessagesController(_loggerMock.Object, _mockMessagesService.Object, _mockUserService.Object);

			//Act
			var result = await controller.AddMessage(ConstIds.ExampleUserId, _messageToAdd);
			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result.Result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockUserService.Verify();
		}
	}
}
