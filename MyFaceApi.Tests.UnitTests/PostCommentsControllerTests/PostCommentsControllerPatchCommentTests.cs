using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyFaceApi.Api.Application.DtoModels.PostComment;
using MyFaceApi.Api.Controllers;
using System;
using Xunit;

namespace MyFaceApi.Api.Tests.UnitTests.PostCommentsControllerTests
{
	public class PostCommentsControllerPatchCommentTests: PostCommentsControllerPreparation
	{
		public PostCommentsControllerPatchCommentTests() : base()
		{
		}
		[Fact]
		public async void PartiallyUpdateComment_ReturnsNoContentResult_WhenTheCommentHasBeenUpdated()
		{
			//Arrange
			_mockCommentService.Setup(Service => Service.TryUpdatePostCommentAsync(It.IsAny<Guid>(), It.IsAny<JsonPatchDocument<PostCommentToUpdateDto>>()))
				.ReturnsAsync(true)
				.Verifiable();

			var controller = new PostCommentsController(_loggerMock.Object, _mockPostService.Object, _mockUserService.Object, _mockCommentService.Object);

			//Act
			var result = await controller.PartiallyUpdateComment(ConstIds.ExampleCommentId, GetJsonPatchDocument());

			//Assert
			var noContentResult = Assert.IsType<NoContentResult>(result);
			_mockCommentService.Verify();
		}
		[Fact]
		public async void PartiallyUpdateComment_ReturnsBadRequestResult_WhenTheCommentHasNotBeenUpdated()
		{
			//Arrange
			_mockCommentService.Setup(Service => Service.TryUpdatePostCommentAsync(It.IsAny<Guid>(), It.IsAny<JsonPatchDocument<PostCommentToUpdateDto>>()))
				.ReturnsAsync(false)
				.Verifiable();

			var controller = new PostCommentsController(_loggerMock.Object, _mockPostService.Object, _mockUserService.Object, _mockCommentService.Object);

			//Act
			var result = await controller.PartiallyUpdateComment(ConstIds.ExampleCommentId, GetJsonPatchDocument());

			//Assert
			var badRequestResult = Assert.IsType<BadRequestResult>(result);
			_mockCommentService.Verify();
		}
		[Fact]
		public async void PartiallyUpdateComment_ReturnsBadRequestObjectResult_WhenTheCommentIdIsInvalid()
		{
			//Arrange
			var controller = new PostCommentsController(_loggerMock.Object, _mockPostService.Object, _mockUserService.Object, _mockCommentService.Object);

			//Act
			var result = await controller.PartiallyUpdateComment(ConstIds.InvalidGuid, GetJsonPatchDocument());

			//Assert
			var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal($"{ConstIds.InvalidGuid} is not valid guid.", badRequestObjectResult.Value);
		}
		[Fact]
		public async void PartiallyUpdateComment_ReturnsInternalServerErrorResult_WhenExceptionThrownInService()
		{
			//Arrange
			_mockCommentService.Setup(Service => Service.TryUpdatePostCommentAsync(It.IsAny<Guid>(), It.IsAny<JsonPatchDocument<PostCommentToUpdateDto>>()))
				.Throws(new ArgumentNullException(nameof(Guid)))
				.Verifiable();

			var controller = new PostCommentsController(_loggerMock.Object, _mockPostService.Object, _mockUserService.Object, _mockCommentService.Object);

			//Act
			var result = await controller.PartiallyUpdateComment(ConstIds.ExampleCommentId, GetJsonPatchDocument());

			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockCommentService.Verify();
		}
	}
}
