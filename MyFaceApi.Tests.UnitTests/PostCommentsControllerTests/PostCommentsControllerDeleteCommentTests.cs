using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyFaceApi.Api.Controllers;
using System;
using Xunit;

namespace MyFaceApi.Tests.UnitTests.PostCommentsControllerTests
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
			_mockCommentService.Setup(Service => Service.DeleteCommentAsync(It.IsAny<Guid>()))
				.Verifiable();


			var controller = new PostCommentsController(_loggerMock.Object, _mockPostService.Object, _mockUserService.Object, _mockCommentService.Object);
			//Act
			var result = await controller.DeleteComment(ConstIds.ExampleCommentId);

			//Act
			var noContentResult = Assert.IsType<NoContentResult>(result);
			_mockCommentService.Verify();
		}
		[Fact]
		public async void DeleteComment_ReturnsBadRequestObjectResult_WhenTheCommentGuidIdIsInvalid()
		{
			//Arrange
			var controller = new PostCommentsController(_loggerMock.Object, _mockPostService.Object, _mockUserService.Object, _mockCommentService.Object);

			//Act
			var result = await controller.DeleteComment(ConstIds.InvalidGuid);

			//Assert
			var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal($"{ConstIds.InvalidGuid} is not valid Guid.", badRequestObjectResult.Value);
		}
		[Fact]
		public async void DeleteComment_ReturnsInternalServerErrorResult_WhenExceptionThrownInService()
		{
			//Arrange
			_mockCommentService.Setup(Service => Service.DeleteCommentAsync(It.IsAny<Guid>()))
				.Throws(new ArgumentNullException(nameof(Guid)))
				.Verifiable();

			var controller = new PostCommentsController(_loggerMock.Object, _mockPostService.Object, _mockUserService.Object, _mockCommentService.Object);
			//Act
			var result = await controller.DeleteComment(ConstIds.ExampleCommentId);

			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockCommentService.Verify();
		}
	}
}
