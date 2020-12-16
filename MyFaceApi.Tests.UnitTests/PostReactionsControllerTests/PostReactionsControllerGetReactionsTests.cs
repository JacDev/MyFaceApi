using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyFaceApi.Api.Application.DtoModels.PostReaction;
using MyFaceApi.Api.Controllers;
using System;
using System.Collections.Generic;
using Xunit;

namespace MyFaceApi.Api.Tests.UnitTests.PostReactionsControllerTests
{
	public class PostReactionsControllerGetReactionsTests : PostReactionsPreparation
	{
		public PostReactionsControllerGetReactionsTests() : base()
		{
		}
		[Fact]
		public void GetPostReactions_ReturnsOkObjectResult_WithAListOfReactionsData()
		{
			//Arrange
			var reactions = GetTestPostData();

			_mockPostService.Setup(Service => Service.CheckIfPostExists(It.IsAny<Guid>()))
				.Returns(true)
				.Verifiable();

			_mockReactionService.Setup(Service => Service.GetPostReactions(It.IsAny<Guid>()))
				.Returns(reactions)
				.Verifiable();

			var controller = new PostReactionsController(_loggerMock.Object, _mockPostService.Object, _mockUserService.Object, _mockReactionService.Object);

			//Act
			var result = controller.GetPostReactions(ConstIds.ExamplePostId);

			//Assert
			var actionResult = Assert.IsType<OkObjectResult>(result.Result);
			var model = Assert.IsType<List<PostReactionDto>>(actionResult.Value);
			Assert.Equal(reactions, model);
			_mockPostService.Verify();
			_mockReactionService.Verify();
		}
		[Fact]
		public void GetPostReactions_ReturnsBadRequestObjectResult_WhenThePostIdIsInvalid()
		{
			//Arrange
			var controller = new PostReactionsController(_loggerMock.Object, _mockPostService.Object, _mockUserService.Object, _mockReactionService.Object); 

			//Act
			var result = controller.GetPostReactions(ConstIds.InvalidGuid);

			//Assert
			var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result.Result);
			Assert.Equal($"{ConstIds.InvalidGuid} is not valid Guid.", badRequestObjectResult.Value);
		}
		[Fact]
		public void GetPostReactions_ReturnsNotFoundObjectResult_WhenThePostDoesntExist()
		{
			//Arrange
			_mockPostService.Setup(Service => Service.CheckIfPostExists(It.IsAny<Guid>()))
				.Returns(false)
				.Verifiable();

			var controller = new PostReactionsController(_loggerMock.Object, _mockPostService.Object, _mockUserService.Object, _mockReactionService.Object);
			//Act
			var result = controller.GetPostReactions(ConstIds.ExamplePostId);

			//Assert
			var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
			Assert.Equal($"Post: {ConstIds.ExamplePostId} doesnt exists.", notFoundResult.Value);
			_mockPostService.Verify();
		}
		[Fact]
		public void GetPostReactions_ReturnsInternalServerErrorResult_WhenExceptionThrownInService()
		{
			//Arrange
			_mockPostService.Setup(Service => Service.CheckIfPostExists(It.IsAny<Guid>()))
				.Throws(new ArgumentNullException(nameof(Guid)))
				.Verifiable();

			var controller = new PostReactionsController(_loggerMock.Object, _mockPostService.Object, _mockUserService.Object, _mockReactionService.Object);
			//Act
			var result = controller.GetPostReactions(ConstIds.ExamplePostId);

			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result.Result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockPostService.Verify();
		}
	}
}

