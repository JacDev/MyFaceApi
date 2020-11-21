using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyFaceApi.Controllers;
using MyFaceApi.DataAccess.Entities;
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
		public void GetComment_ReturnsOkObjectResult_WithCommentData()
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
			var result = controller.GetComment(ConstIds.ExamplePostId, ConstIds.ExampleCommentId);

			//Assert
			var actionResult = Assert.IsType<OkObjectResult>(result.Result);
			var model = Assert.IsType<PostComment>(actionResult.Value);

			_mockCommentRepo.Verify();
			_mockPostRepo.Verify();
		}
		[Theory]
		[InlineData(ConstIds.InvalidGuid, ConstIds.ExampleCommentId)]
		[InlineData(ConstIds.ExamplePostId, ConstIds.InvalidGuid)]
		public void GetComment_ReturnsBadRequestObjectResult_WhenThePostOrCommentIdIsInvalid(string testPostId, string testCommentId)
		{
			//Arrange
			var controller = new PostCommentsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper, _mockCommentRepo.Object);

			//Act
			var result = controller.GetComment(testPostId, testCommentId);

			//Assert
			var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result.Result);
			Assert.Equal($"{testPostId} or {testCommentId} is not valid Guid.", badRequestObjectResult.Value);
		}
		[Theory]
		[InlineData(true, null)]
		[InlineData(false, null)]
		public void GetComment_ReturnsNotFoundObjectResult_WhenThePostDoesntExist(bool isPostExists, PostComment testCommentData)
		{
			//Arrange
			_mockPostRepo.Setup(repo => repo.CheckIfPostExists(It.IsAny<Guid>()))
				.Returns(isPostExists)
				.Verifiable();
			_mockCommentRepo.Setup(repo => repo.GetComment(It.IsAny<Guid>()))
				.Returns(testCommentData)
				.Verifiable();
			var controller = new PostCommentsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper, _mockCommentRepo.Object);

			//Act
			var result = controller.GetComment(ConstIds.ExamplePostId, ConstIds.ExampleCommentId);

			//Assert
			var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
			if (isPostExists)
			{
				Assert.Equal($"Comment: {ConstIds.ExampleCommentId} not found.", notFoundResult.Value);
				_mockCommentRepo.Verify();
			}
			else
			{
				Assert.Equal($"Post: {ConstIds.ExamplePostId} not found.", notFoundResult.Value);
			}
			_mockPostRepo.Verify();
		}
		[Fact]
		public void GetComment_ReturnsInternalServerErrorResult_WhenExceptionThrownInRepository()
		{
			//Arrange
			_mockPostRepo.Setup(repo => repo.CheckIfPostExists(It.IsAny<Guid>()))
				.Throws(new ArgumentNullException(nameof(Guid)))
				.Verifiable();

			var controller = new PostCommentsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper, _mockCommentRepo.Object);

			//Act
			var result = controller.GetComment(ConstIds.ExamplePostId, ConstIds.ExampleCommentId);

			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result.Result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockPostRepo.Verify();
		}
	}
}
