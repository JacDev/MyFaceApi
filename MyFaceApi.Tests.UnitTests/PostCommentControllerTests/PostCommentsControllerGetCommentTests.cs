using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyFaceApi.Controllers;
using MyFaceApi.Entities;
using System;
using System.Linq;
using Xunit;

namespace MyFaceApi.Tests.UnitTests.PostCommentControllerTests
{
	public class PostCommentsControllerGetCommentTests : PostCommentsControllerPreparation
	{
		public PostCommentsControllerGetCommentTests() : base()
		{
		}
		[Fact]
		public void GetComment_ReturnsAnActionResult_WithComment()
		{
			//Arrange
			var comment = GetTestPostData().ElementAt(0);
			_mockPostRepo.Setup(repo => repo.CheckIfPostExists(It.IsAny<Guid>()))
				.Returns(true)
				.Verifiable();
			_mockCommentRepo.Setup(repo => repo.GetComment(It.IsAny<Guid>()))
				.Returns(comment)
				.Verifiable();

			var controller = new PostCommentsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper, _mockCommentRepo.Object);

			//Act
			var result = controller.GetComment(_examplePostId, _exampleCommentId);

			//Assert
			var actionResult = Assert.IsType<OkObjectResult>(result.Result);
			var model = Assert.IsType<PostComment>(actionResult.Value);

			_mockCommentRepo.Verify();
			_mockPostRepo.Verify();
		}
		[Fact]
		public void GetComment_ReturnsBadRequestResult_WhenThePostIdIsInvalid()
		{
			//Arrange
			var controller = new PostCommentsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper, _mockCommentRepo.Object);

			//Act
			var result = controller.GetComment(_invalidGuid, _exampleCommentId);

			//Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
			Assert.Equal($"{_invalidGuid} or {_exampleCommentId} is not valid Guid.", badRequestResult.Value);
		}
		[Fact]
		public void GetComment_ReturnsBadRequestResult_WhenTheCommentIdIsInvalid()
		{
			//Arrange
			var controller = new PostCommentsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper, _mockCommentRepo.Object);

			//Act
			var result = controller.GetComment(_examplePostId, _invalidGuid);

			//Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
			Assert.Equal($"{_examplePostId} or {_invalidGuid} is not valid Guid.", badRequestResult.Value);
		}
		[Fact]
		public void GetComment_ReturnsNotFoundObjectResult_WhenThePostDoesntExist()
		{
			//Arrange
			_mockPostRepo.Setup(repo => repo.CheckIfPostExists(It.IsAny<Guid>()))
				.Returns(false)
				.Verifiable();

			var controller = new PostCommentsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper, _mockCommentRepo.Object);

			//Act
			var result = controller.GetComment(_examplePostId, _exampleCommentId);

			//Assert
			var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
			Assert.Equal($"Post: {_examplePostId} not found.", notFoundResult.Value);
			_mockPostRepo.Verify();
		}
		[Fact]
		public void GetComment_ReturnsNotFoundObjectResult_WhenTheCommentDoesntExist()
		{
			//Arrange
			_mockPostRepo.Setup(repo => repo.CheckIfPostExists(It.IsAny<Guid>()))
				.Returns(true)
				.Verifiable();
			_mockCommentRepo.Setup(repo => repo.GetComment(It.IsAny<Guid>()))
				.Returns((PostComment)null)
				.Verifiable();

			var controller = new PostCommentsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper, _mockCommentRepo.Object);

			//Act
			var result = controller.GetComment(_examplePostId, _exampleCommentId);

			//Assert
			var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
			Assert.Equal($"Comment: {_exampleCommentId} not found.", notFoundResult.Value);
			_mockPostRepo.Verify();
			_mockCommentRepo.Verify();
		}
		[Fact]
		public void GetComment_ReturnsInternalServerError_WhenExceptionThrownInRepository()
		{
			//Arrange
			_mockPostRepo.Setup(repo => repo.CheckIfPostExists(It.IsAny<Guid>()))
				.Throws(new ArgumentNullException(nameof(_examplePostId)))
				.Verifiable();

			var controller = new PostCommentsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper, _mockCommentRepo.Object);

			//Act
			var result = controller.GetComment(_examplePostId, _exampleCommentId);

			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result.Result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockPostRepo.Verify();
		}
	}
}
