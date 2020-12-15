using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyFaceApi.Api.Application.DtoModels.PostComment;
using MyFaceApi.Api.Controllers;
using System;
using System.Linq;
using Xunit;

namespace MyFaceApi.Tests.UnitTests.PostCommentsControllerTests
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
			_mockPostService.Setup(Service => Service.CheckIfPostExists(It.IsAny<Guid>()))
				.Returns(true)
				.Verifiable();
			_mockCommentService.Setup(Service => Service.GetComment(It.IsAny<Guid>()))
				.Returns(comment)
				.Verifiable();

			var controller = new PostCommentsController(_loggerMock.Object, _mockPostService.Object, _mockUserService.Object, _mockCommentService.Object);

			//Act
			var result = controller.GetComment(ConstIds.ExamplePostId, ConstIds.ExampleCommentId);

			//Assert
			var actionResult = Assert.IsType<OkObjectResult>(result.Result);
			var model = Assert.IsType<PostCommentDto>(actionResult.Value);

			_mockCommentService.Verify();
			_mockPostService.Verify();
		}
		[Theory]
		[InlineData(ConstIds.InvalidGuid, ConstIds.ExampleCommentId)]
		[InlineData(ConstIds.ExamplePostId, ConstIds.InvalidGuid)]
		public void GetComment_ReturnsBadRequestObjectResult_WhenThePostOrCommentIdIsInvalid(string testPostId, string testCommentId)
		{
			//Arrange
			var controller = new PostCommentsController(_loggerMock.Object, _mockPostService.Object, _mockUserService.Object, _mockCommentService.Object);

			//Act
			var result = controller.GetComment(testPostId, testCommentId);

			//Assert
			var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result.Result);
			Assert.Equal($"{testPostId} or {testCommentId} is not valid guid.", badRequestObjectResult.Value);
		}
		[Fact]
		public void GetComment_ReturnsNotFoundObjectResult_WhenThePostDoesntExist()
		{
			//Arrange
			_mockPostService.Setup(Service => Service.CheckIfPostExists(It.IsAny<Guid>()))
				.Returns(false)
				.Verifiable();

			var controller = new PostCommentsController(_loggerMock.Object, _mockPostService.Object, _mockUserService.Object, _mockCommentService.Object);

			//Act
			var result = controller.GetComment(ConstIds.ExamplePostId, ConstIds.ExampleCommentId);

			//Assert
			var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
			Assert.Equal($"Post: {ConstIds.ExamplePostId} not found.", notFoundResult.Value);
			_mockPostService.Verify();
		}
		[Fact]
		public void GetComment_ReturnsInternalServerErrorResult_WhenExceptionThrownInService()
		{
			//Arrange
			_mockPostService.Setup(Service => Service.CheckIfPostExists(It.IsAny<Guid>()))
				.Throws(new ArgumentNullException(nameof(Guid)))
				.Verifiable();

			var controller = new PostCommentsController(_loggerMock.Object, _mockPostService.Object, _mockUserService.Object, _mockCommentService.Object);

			//Act
			var result = controller.GetComment(ConstIds.ExamplePostId, ConstIds.ExampleCommentId);

			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result.Result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockPostService.Verify();
		}
	}
}
