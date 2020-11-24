using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyFaceApi.Api.Controllers;
using MyFaceApi.Api.DataAccess.Entities;
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
			var reaction = GetTestPostData().PostReactions.ElementAt(0);

			_mockPostRepo.Setup(repo => repo.CheckIfPostExists(It.IsAny<Guid>()))
				.Returns(true)
				.Verifiable();

			_mockReactionRepo.Setup(repo => repo.GetPostReaction(It.IsAny<Guid>()))
				.Returns(reaction)
				.Verifiable();

			var controller = new PostReactionsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper, _mockReactionRepo.Object);

			//Act
			var result = controller.GetPostReaction(ConstIds.ExamplePostId, ConstIds.ExampleReactionId);

			//Assert
			var actionResult = Assert.IsType<OkObjectResult>(result.Result);
			var model = Assert.IsType<PostReaction>(actionResult.Value);
			Assert.Equal(reaction, model);
			_mockPostRepo.Verify();
			_mockReactionRepo.Verify();
		}
		[Theory]
		[InlineData(ConstIds.InvalidGuid, ConstIds.ExampleReactionId)]
		[InlineData(ConstIds.ExamplePostId, ConstIds.InvalidGuid)]
		public void GetPostReaction_ReturnsBadRequestObjectResult_WhenThePostOrReactionIdIsInvalid(string testPostId, string testReactionId)
		{
			//Arrange
			var controller = new PostReactionsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper, _mockReactionRepo.Object);
			//Act
			var result = controller.GetPostReaction(testPostId, testReactionId);

			//Assert
			var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result.Result);
			Assert.Equal($"{testPostId} or {testReactionId} is not valid Guid.", badRequestObjectResult.Value);
		}
		[Theory]
		[InlineData(true, null)]
		[InlineData(false, null)]
		public void GetPostReaction_ReturnsNotFoundObjectResult_WhenTheReactionDoesntExist(bool doesThePostExists, PostReaction testPostReaction)
		{
			//Arrange
			_mockPostRepo.Setup(repo => repo.CheckIfPostExists(It.IsAny<Guid>()))
				.Returns(doesThePostExists)
				.Verifiable();

			_mockReactionRepo.Setup(repo => repo.GetPostReaction(It.IsAny<Guid>()))
				.Returns(testPostReaction)
				.Verifiable();

			var controller = new PostReactionsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper, _mockReactionRepo.Object);

			//Act
			var result = controller.GetPostReaction(ConstIds.ExamplePostId, ConstIds.ExampleReactionId);

			//Assert
			var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result.Result);
			if (doesThePostExists)
			{
				Assert.Equal($"Reaction: {ConstIds.ExampleReactionId} not found.", notFoundObjectResult.Value);
				_mockReactionRepo.Verify();
			}
			else
			{
				Assert.Equal($"Post: {ConstIds.ExamplePostId} not found.", notFoundObjectResult.Value);
			}
			
			_mockPostRepo.Verify();
		}
		
		[Fact]
		public void GetPostReaction_ReturnsInternalServerErrorResult_WhenExceptionThrownInRepository()
		{
			//Arrange
			_mockPostRepo.Setup(repo => repo.CheckIfPostExists(It.IsAny<Guid>()))
				.Throws(new ArgumentNullException(nameof(Guid)))
				.Verifiable();

			var controller = new PostReactionsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper, _mockReactionRepo.Object);

			//Act
			var result = controller.GetPostReaction(ConstIds.ExamplePostId, ConstIds.ExampleReactionId);

			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result.Result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockPostRepo.Verify();
		}
	}
}
