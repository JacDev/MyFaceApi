using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyFaceApi.Api.Application.DtoModels.PostReaction;
using MyFaceApi.Api.Controllers;
using System;
using System.Linq;
using Xunit;

namespace MyFaceApi.Tests.UnitTests.PostReactionsControllerTests
{
	public class PostReactionsControllerGetReactionTests : PostReactionsPreparation
	{
		public PostReactionsControllerGetReactionTests() : base()
		{
		}
		[Fact]
		public void GetPostReaction_ReturnsOkObjectResult_WithReactionData()
		{
			//Arrange
			var reaction = GetTestPostData().ElementAt(0);

			_mockPostService.Setup(Service => Service.CheckIfPostExists(It.IsAny<Guid>()))
				.Returns(true)
				.Verifiable();

			_mockReactionService.Setup(Service => Service.GetPostReaction(It.IsAny<Guid>()))
				.Returns(reaction)
				.Verifiable();

			var controller = new PostReactionsController(_loggerMock.Object, _mockPostService.Object, _mockUserService.Object, _mockReactionService.Object);
			//Act
			var result = controller.GetPostReaction(ConstIds.ExamplePostId, ConstIds.ExampleReactionId);

			//Assert
			var actionResult = Assert.IsType<OkObjectResult>(result.Result);
			var model = Assert.IsType<PostReactionDto>(actionResult.Value);
			Assert.Equal(reaction, model);
			_mockPostService.Verify();
			_mockReactionService.Verify();
		}
		[Theory]
		[InlineData(ConstIds.InvalidGuid, ConstIds.ExampleReactionId)]
		[InlineData(ConstIds.ExamplePostId, ConstIds.InvalidGuid)]
		public void GetPostReaction_ReturnsBadRequestObjectResult_WhenThePostOrReactionIdIsInvalid(string testPostId, string testReactionId)
		{
			//Arrange
			var controller = new PostReactionsController(_loggerMock.Object, _mockPostService.Object, _mockUserService.Object, _mockReactionService.Object);  
			//Act
			var result = controller.GetPostReaction(testPostId, testReactionId);

			//Assert
			var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result.Result);
			Assert.Equal($"{testPostId} or {testReactionId} is not valid guid.", badRequestObjectResult.Value);
		}
		[Fact]
		public void GetPostReaction_ReturnsNotFoundObjectResult_WhenThePostDoesntExist()
		{
			//Arrange
			_mockPostService.Setup(Service => Service.CheckIfPostExists(It.IsAny<Guid>()))
				.Returns(false)
				.Verifiable();

			var controller = new PostReactionsController(_loggerMock.Object, _mockPostService.Object, _mockUserService.Object, _mockReactionService.Object);

			//Act
			var result = controller.GetPostReaction(ConstIds.ExamplePostId, ConstIds.ExampleReactionId);

			//Assert
			var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result.Result);
			Assert.Equal($"Post: {ConstIds.ExamplePostId} not found.", notFoundObjectResult.Value);
			
			_mockPostService.Verify();
		}		
		[Fact]
		public void GetPostReaction_ReturnsInternalServerErrorResult_WhenExceptionThrownInService()
		{
			//Arrange
			_mockPostService.Setup(Service => Service.CheckIfPostExists(It.IsAny<Guid>()))
				.Throws(new ArgumentNullException(nameof(Guid)))
				.Verifiable();

			var controller = new PostReactionsController(_loggerMock.Object, _mockPostService.Object, _mockUserService.Object, _mockReactionService.Object);

			//Act
			var result = controller.GetPostReaction(ConstIds.ExamplePostId, ConstIds.ExampleReactionId);

			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result.Result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockPostService.Verify();
		}
	}
}
