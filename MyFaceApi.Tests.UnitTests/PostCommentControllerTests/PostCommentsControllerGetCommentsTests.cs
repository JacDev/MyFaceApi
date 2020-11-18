using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyFaceApi.Controllers;
using MyFaceApi.Entities;
using System;
using System.Collections.Generic;
using Xunit;

namespace MyFaceApi.Tests.UnitTests.PostCommentControllerTests
{
	public class PostCommentsControllerGetCommentsTests : PostCommentsControllerPreparation
	{
		public PostCommentsControllerGetCommentsTests() : base()
		{
		}
		[Fact]
		public void GetComments_ReturnsAnActionResult_WithListOfComments()
		{
			//Arrange
			var comments = GetTestPostData();
			_mockPostRepo.Setup(repo => repo.CheckIfPostExists(It.IsAny<Guid>()))
				.Returns(true)
				.Verifiable();
			_mockCommentRepo.Setup(repo => repo.GetComments(It.IsAny<Guid>()))
				.Returns(comments)
				.Verifiable();

			var controller = new PostCommentsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper, _mockCommentRepo.Object);

			//Act
			var result = controller.GetComments(_examplePostId);

			//Assert
			var actionResult = Assert.IsType<OkObjectResult>(result.Result);
			var model = Assert.IsType<List<PostComment>>(actionResult.Value);

			_mockCommentRepo.Verify();
			_mockPostRepo.Verify();
		}
		[Fact]
		public void GetComments_ReturnsBadRequestResult_WhenThePostIdIsInvalid()
		{
			//Arrange
			var controller = new PostCommentsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper, _mockCommentRepo.Object);

			//Act
			var result = controller.GetComments(_invalidGuid);

			//Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
			Assert.Equal($"{_invalidGuid} is not valid Guid.", badRequestResult.Value);
		}
		[Fact]
		public void GetComments_ReturnsNotFoundObjectResult_WhenThePostDoesntExist()
		{
			//Arrange
			_mockPostRepo.Setup(repo => repo.CheckIfPostExists(It.IsAny<Guid>()))
				.Returns(false)
				.Verifiable();

			var controller = new PostCommentsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper, _mockCommentRepo.Object);

			//Act
			var result = controller.GetComments(_examplePostId);

			//Assert
			var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
			Assert.Equal($"Post: {_examplePostId} not found.", notFoundResult.Value);
			_mockPostRepo.Verify();
		}
		[Fact]
		public void GetComments_ReturnsInternalServerError_WhenExceptionThrownInRepository()
		{
			//Arrange
			_mockPostRepo.Setup(repo => repo.CheckIfPostExists(It.IsAny<Guid>()))
				.Throws(new ArgumentNullException(nameof(_examplePostId)))
				.Verifiable();

			var controller = new PostCommentsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper, _mockCommentRepo.Object);

			//Act
			var result = controller.GetComments(_examplePostId);

			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result.Result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockPostRepo.Verify();
		}
	}
}
