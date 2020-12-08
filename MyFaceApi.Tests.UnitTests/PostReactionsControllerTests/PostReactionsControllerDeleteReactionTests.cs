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
	public class PostReactionsControllerDeleteReactionTests : PostReactionsPreparation
	{
		public PostReactionsControllerDeleteReactionTests() : base()
		{
		}
		[Fact]
		public async void DeletePostReaction_ReturnsNoContentResult_WhenThePostHasBeenRemoved()
		{
			//Arrange
			var reactionToRemove = GetTestPostData().PostReactions.ElementAt(0);
			_mockReactionRepo.Setup(repo => repo.GetPostReaction(It.IsAny<Guid>()))
				.Returns(reactionToRemove)
				.Verifiable();
			_mockReactionRepo.Setup(repo => repo.DeletePostReactionAsync(It.IsAny<PostReaction>()))
				.Verifiable();

			var controller = new PostReactionsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper, _mockReactionRepo.Object);

			//Act
			var result = await controller.DeletePostReaction(ConstIds.ExampleReactionId, ConstIds.ExampleFromWhoId);

			//Act
			var noContentResult = Assert.IsType<NoContentResult>(result);
			_mockReactionRepo.Verify();
		}
		[Fact]
		public async void DeletePostReaction_ReturnsBadRequestObjectResult_WhenThePostReactionGuidIdIsInvalid()
		{
			//Arrange
			var controller = new PostReactionsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper, _mockReactionRepo.Object);

			//Act
			var result = await controller.DeletePostReaction(ConstIds.InvalidGuid, ConstIds.ExampleFromWhoId);

			//Assert
			var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal($"{ConstIds.InvalidGuid} is not valid Guid.", badRequestObjectResult.Value);
		}
		[Fact]
		public async void DeletePostReaction_ReturnsNotFoundResult_WhenTheReactionDoesntExist()
		{
			//Arrange
			_mockReactionRepo.Setup(repo => repo.GetPostReaction(It.IsAny<Guid>()))
				.Returns((PostReaction)null)
				.Verifiable();

			var controller = new PostReactionsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper, _mockReactionRepo.Object);

			//Act
			var result = await controller.DeletePostReaction(ConstIds.ExampleReactionId, ConstIds.ExampleFromWhoId);

			//Assert
			var notFoundResult = Assert.IsType<NotFoundResult>(result);
			_mockReactionRepo.Verify();
		}
		[Fact]
		public async void DeletePostReaction_ReturnsInternalServerErrorResult_WhenExceptionThrownInRepository()
		{
			//Arrange
			_mockReactionRepo.Setup(repo => repo.GetPostReaction(It.IsAny<Guid>()))
				.Throws(new ArgumentNullException(nameof(Guid)))
				.Verifiable();

			var controller = new PostReactionsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper, _mockReactionRepo.Object);

			//Act
			var result = await controller.DeletePostReaction(ConstIds.ExampleReactionId, ConstIds.ExampleFromWhoId);

			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockReactionRepo.Verify();
		}
	}
}
