using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyFaceApi.Api.Controllers;
using System;
using Xunit;

namespace MyFaceApi.Tests.UnitTests.PostReactionsControllerTests
{
	public class PostReactionsControllerDeleteReactionTests : PostReactionsPreparation
	{
		public PostReactionsControllerDeleteReactionTests() : base()
		{
		}
		[Fact]
		public async void DeletePostReaction_ReturnsNoContentResult_WhenThePostHasBeenRemoved()
		{
			//Arrange
			_mockReactionService.Setup(Service => Service.DeletePostReactionAsync(It.IsAny<Guid>()))
				.Verifiable();

			var controller = new PostReactionsController(_loggerMock.Object, _mockPostService.Object, _mockUserService.Object, _mockReactionService.Object);

			//Act
			var result = await controller.DeletePostReaction(ConstIds.ExampleReactionId, ConstIds.ExampleFromWhoId);

			//Act
			var noContentResult = Assert.IsType<NoContentResult>(result);
			_mockReactionService.Verify();
		}
		[Fact]
		public async void DeletePostReaction_ReturnsBadRequestObjectResult_WhenThePostReactionGuidIdIsInvalid()
		{
			//Arrange
			var controller = new PostReactionsController(_loggerMock.Object, _mockPostService.Object, _mockUserService.Object, _mockReactionService.Object);
			//Act
			var result = await controller.DeletePostReaction(ConstIds.InvalidGuid, ConstIds.ExampleFromWhoId);

			//Assert
			var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal($"{ConstIds.InvalidGuid} is not valid Guid.", badRequestObjectResult.Value);
		}
		[Theory]
		[InlineData(true, false)]
		[InlineData(false, true)]
		public async void DeletePostReaction_ReturnsNotFoundResult_WhenTheUserOrPostDoesntyExist(bool doesTheUserExists, bool doesThePostExists)
		{
			//Arrange
			_mockPostService.Setup(Service => Service.CheckIfPostExists(It.IsAny<Guid>()))
				.Returns(doesThePostExists)
				.Verifiable();
			_mockUserService.Setup(Service => Service.CheckIfUserExists(It.IsAny<Guid>()))
				.ReturnsAsync(doesTheUserExists)
				.Verifiable();

			var controller = new PostReactionsController(_loggerMock.Object, _mockPostService.Object, _mockUserService.Object, _mockReactionService.Object);
			//Act
			var result = await controller.DeletePostReaction(ConstIds.ExampleReactionId, ConstIds.ExampleFromWhoId);

			//Assert
			var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result);
			Assert.Equal($"User: {ConstIds.ExampleUserId} or post {ConstIds.ExamplePostId} not found.", notFoundObjectResult.Value);
			_mockPostService.Verify();
			//if post doesnt exist the user will not be checked
			if (doesThePostExists)
			{
				_mockUserService.Verify();
			}
		}
		[Theory]
		[InlineData(ConstIds.ExampleUserId, ConstIds.InvalidGuid)]
		[InlineData(ConstIds.InvalidGuid, ConstIds.ExamplePostId)]
		public async void DeletePostReaction_ReturnsBadRequestObjectResult_WhenThePostOrUserIdIsInvalid(string userTestId, string postTestId)
		{
			//Arrange
			var controller = new PostReactionsController(_loggerMock.Object, _mockPostService.Object, _mockUserService.Object, _mockReactionService.Object);
			//Act
			var result = await controller.DeletePostReaction(ConstIds.ExampleReactionId, ConstIds.ExampleFromWhoId);

			//Assert
			var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal($"{userTestId} or {postTestId} is not valid Guid.", badRequestObjectResult.Value);
		}
		[Fact]
		public async void DeletePostReaction_ReturnsInternalServerErrorResult_WhenExceptionThrownInService()
		{
			//Arrange
			_mockReactionService.Setup(Service => Service.DeletePostReactionAsync(It.IsAny<Guid>()))
				.Throws(new ArgumentNullException(nameof(Guid)))
				.Verifiable();

			var controller = new PostReactionsController(_loggerMock.Object, _mockPostService.Object, _mockUserService.Object, _mockReactionService.Object);
			//Act
			var result = await controller.DeletePostReaction(ConstIds.ExampleReactionId, ConstIds.ExampleFromWhoId);

			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockReactionService.Verify();
		}
	}
}
