using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyFaceApi.Api.Application.DtoModels.PostReaction;
using MyFaceApi.Api.Controllers;
using System;
using Xunit;

namespace MyFaceApi.Api.Tests.UnitTests.PostReactionsControllerTests
{
	public class PostReactionsControllerPatchReactionTests : PostReactionsPreparation
	{
		public PostReactionsControllerPatchReactionTests() : base()
		{
		}
		[Fact]
		public async void PartiallyUpdatePostReaction_ReturnsNoContentResult_WhenTheReactionHasBeenUpdated()
		{
			//Arrange
			_mockReactionService.Setup(Service => Service.TryUpdatePostReactionAsync(It.IsAny<Guid>(), It.IsAny<JsonPatchDocument<PostReactionToUpdateDto>>()))
				.ReturnsAsync(true)
				.Verifiable();

			var controller = new PostReactionsController(_loggerMock.Object, _mockPostService.Object, _mockUserService.Object, _mockReactionService.Object);

			//Act
			var result = await controller.PartiallyUpdatePostReaction(ConstIds.ExampleReactionId, GetJsonPatchDocument());

			//Assert
			var noContentResult = Assert.IsType<NoContentResult>(result);
			_mockReactionService.Verify();
		}
		[Fact]
		public async void PartiallyUpdatePostReaction_ReturnsBadRequestResult_WhenTheReactionHasNotBeenUpdated()
		{
			//Arrange
			_mockReactionService.Setup(Service => Service.TryUpdatePostReactionAsync(It.IsAny<Guid>(), It.IsAny<JsonPatchDocument<PostReactionToUpdateDto>>()))
				.ReturnsAsync(false)
				.Verifiable();

			var controller = new PostReactionsController(_loggerMock.Object, _mockPostService.Object, _mockUserService.Object, _mockReactionService.Object);

			//Act
			var result = await controller.PartiallyUpdatePostReaction(ConstIds.ExampleReactionId, GetJsonPatchDocument());

			//Assert
			var badRequestResult = Assert.IsType<BadRequestResult>(result);
			_mockReactionService.Verify();
		}
		[Fact]
		public async void PartiallyUpdatePostReaction_ReturnsBadRequestObjectResult_WhenTheReactionIdIsInvalid()
		{
			//Arrange
			var controller = new PostReactionsController(_loggerMock.Object, _mockPostService.Object, _mockUserService.Object, _mockReactionService.Object);

			//Act
			var result = await controller.PartiallyUpdatePostReaction(ConstIds.InvalidGuid, GetJsonPatchDocument());

			//Assert
			var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal($"{ConstIds.InvalidGuid} is not valid guid.", badRequestObjectResult.Value);
		}
		[Fact]
		public async void PartiallyUpdatePostReaction_ReturnsInternalServerErrorResult_WhenExceptionThrownInService()
		{
			//Arrange
			_mockReactionService.Setup(Service => Service.TryUpdatePostReactionAsync(It.IsAny<Guid>(), It.IsAny<JsonPatchDocument<PostReactionToUpdateDto>>()))
				.Throws(new ArgumentNullException(nameof(Guid)))
				.Verifiable();

			var controller = new PostReactionsController(_loggerMock.Object, _mockPostService.Object, _mockUserService.Object, _mockReactionService.Object);

			//Act
			var result = await controller.PartiallyUpdatePostReaction(ConstIds.ExampleReactionId, GetJsonPatchDocument());

			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockReactionService.Verify();
		}
	}
}
