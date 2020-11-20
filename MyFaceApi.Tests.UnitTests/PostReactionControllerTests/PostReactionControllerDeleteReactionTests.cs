using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyFaceApi.Controllers;
using MyFaceApi.Entities;
using System;
using System.Linq;
using Xunit;

namespace MyFaceApi.Tests.UnitTests.PostReactionControllerTests
{
	public class PostReactionControllerDeleteReactionTests : PostReactionPreparation
	{
		public PostReactionControllerDeleteReactionTests() : base()
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

			var controller = new PostReactionController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper, _mockReactionRepo.Object);

			//Act
			var result = await controller.DeletePostReaction(ConstIds.ExampleReactionId);

			//Act
			var noContentResult = Assert.IsType<NoContentResult>(result);
			_mockReactionRepo.Verify();
		}
		[Fact]
		public async void DeletePostReaction_ReturnsBadRequestObjectResult_WhenThePostReactionGuidIdIsInvalid()
		{
			//Arrange
			var controller = new PostReactionController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper, _mockReactionRepo.Object);

			//Act
			var result = await controller.DeletePostReaction(ConstIds.InvalidGuid);

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

			var controller = new PostReactionController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper, _mockReactionRepo.Object);

			//Act
			var result = await controller.DeletePostReaction(ConstIds.ExampleReactionId);

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

			var controller = new PostReactionController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper, _mockReactionRepo.Object);

			//Act
			var result = await controller.DeletePostReaction(ConstIds.ExampleReactionId);

			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockReactionRepo.Verify();
		}
	}
}
