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
	public class PostCommentsControllerDeleteCommentTests : PostCommentsControllerPreparation
	{
		public PostCommentsControllerDeleteCommentTests() : base()
		{
		}
		[Fact]
		public async void DeleteComment_ReturnsNoContentResult_WhenTheCommentHasBeenRemoved()
		{
			//Arrange
			var commentToRemove = GetTestPostData().ElementAt(0);
			_mockCommentRepo.Setup(repo => repo.GetComment(It.IsAny<Guid>()))
				.Returns(commentToRemove)
				.Verifiable();

			_mockCommentRepo.Setup(repo => repo.DeleteCommentAsync(It.IsAny<PostComment>()))
				.Verifiable();

			var controller = new PostCommentsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper, _mockCommentRepo.Object);
			//Act
			var result = await controller.DeleteComment(ConstIds.ExampleCommentId);

			//Act
			var noContentResult = Assert.IsType<NoContentResult>(result);
			_mockCommentRepo.Verify();
		}
		[Fact]
		public async void DeleteComment_ReturnsBadRequestObjectResult_WhenTheCommentGuidIdIsInvalid()
		{
			//Arrange
			var controller = new PostCommentsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper, _mockCommentRepo.Object);

			//Act
			var result = await controller.DeleteComment(ConstIds.InvalidGuid);

			//Assert
			var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal($"{ConstIds.InvalidGuid} is not valid Guid.", badRequestObjectResult.Value);
		}
		[Fact]
		public async void DeleteComment_ReturnsNotFoundResult_WhenTheCommentDoesntExist()
		{
			//Arrange
			_mockCommentRepo.Setup(repo => repo.GetComment(It.IsAny<Guid>()))
				.Returns((PostComment)null)
				.Verifiable();

			var controller = new PostCommentsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper, _mockCommentRepo.Object);
			//Act
			var result = await controller.DeleteComment(ConstIds.ExampleCommentId);

			//Assert
			var notFoundResult = Assert.IsType<NotFoundResult>(result);
			_mockCommentRepo.Verify();
		}
		[Fact]
		public async void DeleteComment_ReturnsInternalServerErrorResult_WhenExceptionThrownInRepository()
		{
			//Arrange
			_mockCommentRepo.Setup(repo => repo.GetComment(It.IsAny<Guid>()))
				.Throws(new ArgumentNullException(nameof(Guid)))
				.Verifiable();

			var controller = new PostCommentsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper, _mockCommentRepo.Object);
			//Act
			var result = await controller.DeleteComment(ConstIds.ExampleCommentId);

			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockCommentRepo.Verify();
		}
	}
}
