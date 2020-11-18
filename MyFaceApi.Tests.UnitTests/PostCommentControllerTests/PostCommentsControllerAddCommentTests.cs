using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyFaceApi.Controllers;
using MyFaceApi.Entities;
using System;
using Xunit;

namespace MyFaceApi.Tests.UnitTests.PostCommentControllerTests
{
	public class PostCommentsControllerAddCommentTests : PostCommentsControllerPreparation
	{
		public PostCommentsControllerAddCommentTests() : base()
		{
		}
		[Fact]
		public async void AddComment_ReturnsRedirectToAddedUser_WithPostComment()
		{
			//Arrange
			_mockPostRepo.Setup(repo => repo.CheckIfPostExists(It.IsAny<Guid>()))
				.Returns(true)
				.Verifiable();
			_mockUserRepo.Setup(repo => repo.CheckIfUserExists(It.IsAny<Guid>()))
				.Returns(true)
				.Verifiable();

			PostComment commentEntity = _mapper.Map<PostComment>(_commentToAdd);
			commentEntity.Id = new Guid(_exampleCommentId);

			_mockCommentRepo.Setup(repo => repo.AddCommentAsync(It.IsAny<PostComment>()))
				.ReturnsAsync(commentEntity)
				.Verifiable();

			var controller = new PostCommentsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper, _mockCommentRepo.Object);

			//Act
			var result = await controller.AddComment(_exampleUserId, _examplePostId, _commentToAdd);

			//Assert
			var redirectToActionResult = Assert.IsType<CreatedAtRouteResult>(result.Result);
			Assert.Equal(_exampleUserId.ToLower(), redirectToActionResult.RouteValues["userId"].ToString().ToLower());
			Assert.Equal(_examplePostId.ToLower(), redirectToActionResult.RouteValues["postId"].ToString().ToLower());
			Assert.Equal(_exampleCommentId.ToLower(), redirectToActionResult.RouteValues["commentId"].ToString().ToLower());
			Assert.Equal("GetComment", redirectToActionResult.RouteName);
			_mockCommentRepo.Verify();
			_mockPostRepo.Verify();
			_mockUserRepo.Verify();
		}
		[Fact]
		public async void AddComment_ReturnsBadRequestResult_WhenThePostIdIsInvalid()
		{
			//Arrange
			var controller = new PostCommentsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper, _mockCommentRepo.Object);

			//Act
			var result = await controller.AddComment(_exampleUserId, _invalidGuid, _commentToAdd);

			//Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
			Assert.Equal($"{_invalidGuid} or {_exampleUserId} is not valid Guid.", badRequestResult.Value);
		}
		[Fact]
		public async void AddComment_ReturnsBadRequestResult_WhenTheUserIdIsInvalid()
		{
			//Arrange
			var controller = new PostCommentsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper, _mockCommentRepo.Object);

			//Act
			var result = await controller.AddComment(_invalidGuid, _examplePostId, _commentToAdd);

			//Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
			Assert.Equal($"{_examplePostId} or {_invalidGuid} is not valid Guid.", badRequestResult.Value);
		}
		[Fact]
		public async void AddComment_ReturnsNotFoundObjectResult_WhenThePostDoesntExist()
		{
			//Arrange
			_mockPostRepo.Setup(repo => repo.CheckIfPostExists(It.IsAny<Guid>()))
				.Returns(false)
				.Verifiable();

			var controller = new PostCommentsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper, _mockCommentRepo.Object);

			//Act
			var result = await controller.AddComment(_exampleUserId, _examplePostId, _commentToAdd);

			//Assert
			var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
			Assert.Equal($"Post: {_examplePostId} or user {_exampleUserId} not found.", notFoundResult.Value);
			_mockPostRepo.Verify();
		}
		[Fact]
		public async void AddComment_ReturnsNotFoundObjectResult_WhenTheUserDoesntExist()
		{
			//Arrange
			_mockPostRepo.Setup(repo => repo.CheckIfPostExists(It.IsAny<Guid>()))
				.Returns(true)
				.Verifiable();
			_mockUserRepo.Setup(repo => repo.CheckIfUserExists(It.IsAny<Guid>()))
				.Returns(false)
				.Verifiable();

			var controller = new PostCommentsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper, _mockCommentRepo.Object);

			//Act
			var result = await controller.AddComment(_exampleUserId, _examplePostId, _commentToAdd);

			//Assert
			var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
			Assert.Equal($"Post: {_examplePostId} or user {_exampleUserId} not found.", notFoundResult.Value);
			_mockUserRepo.Verify();
			_mockPostRepo.Verify();
		}
		[Fact]
		public async void AddComment_ReturnsInternalServerError_WhenExceptionThrownInRepository()
		{
			//Arrange
			_mockPostRepo.Setup(repo => repo.CheckIfPostExists(It.IsAny<Guid>()))
				.Throws(new ArgumentNullException(nameof(_examplePostId)))
				.Verifiable();

			var controller = new PostCommentsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper, _mockCommentRepo.Object);

			//Act
			var result = await controller.AddComment(_exampleUserId, _examplePostId, _commentToAdd);

			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result.Result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockPostRepo.Verify();
		}
	}
}
