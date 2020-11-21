using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyFaceApi.DataAccess.Entities;
using Moq;
using MyFaceApi.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace MyFaceApi.Tests.UnitTests.PostReactionsControllerTests
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
			var reactions = GetTestPostData().PostReactions.ToList();

			_mockPostRepo.Setup(repo => repo.CheckIfPostExists(It.IsAny<Guid>()))
				.Returns(true)
				.Verifiable();

			_mockReactionRepo.Setup(repo => repo.GetPostReactions(It.IsAny<Guid>()))
				.Returns(reactions)
				.Verifiable();

			var controller = new PostReactionsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper, _mockReactionRepo.Object);

			//Act
			var result = controller.GetPostReactions(ConstIds.ExamplePostId);

			//Assert
			var actionResult = Assert.IsType<OkObjectResult>(result.Result);
			var model = Assert.IsType<List<PostReaction>>(actionResult.Value);
			Assert.Equal(reactions, model);
			_mockPostRepo.Verify();
			_mockReactionRepo.Verify();
		}
		[Fact]
		public void GetPostReactions_ReturnsBadRequestObjectResult_WhenThePostIdIsInvalid()
		{
			//Arrange
			var controller = new PostReactionsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper, _mockReactionRepo.Object);
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
			_mockPostRepo.Setup(repo => repo.CheckIfPostExists(It.IsAny<Guid>()))
				.Returns(false)
				.Verifiable();

			var controller = new PostReactionsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper, _mockReactionRepo.Object);

			//Act
			var result = controller.GetPostReactions(ConstIds.ExamplePostId);

			//Assert
			var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
			Assert.Equal($"Post: {ConstIds.ExamplePostId} not found.", notFoundResult.Value);
			_mockPostRepo.Verify();
		}
		[Fact]
		public void GetPostReactions_ReturnsInternalServerErrorResult_WhenExceptionThrownInRepository()
		{
			//Arrange
			_mockPostRepo.Setup(repo => repo.CheckIfPostExists(It.IsAny<Guid>()))
				.Throws(new ArgumentNullException(nameof(Guid)))
				.Verifiable();

			var controller = new PostReactionsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper, _mockReactionRepo.Object);

			//Act
			var result = controller.GetPostReactions(ConstIds.ExamplePostId);

			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result.Result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockPostRepo.Verify();
		}
	}
}

