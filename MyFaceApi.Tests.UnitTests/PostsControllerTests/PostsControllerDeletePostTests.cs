using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyFaceApi.Api.Controllers;
using System;
using Xunit;

namespace MyFaceApi.Tests.UnitTests.PostsControllerTests
{
	public class PostsControllerDeletePostTests : PostsControllerPreparation
	{
		public PostsControllerDeletePostTests() : base()
		{
		}
		[Fact]
		public async void DeletePost_ReturnsNoContentResult_WhenThePostHasBeenRemoved()
		{
			//Arrange
			_mockPostService.Setup(Service => Service.DeletePostAsync(It.IsAny<Guid>()))
				.Verifiable();

			var controller = new PostsController(_loggerMock.Object, _mockPostService.Object, _mockUserService.Object);
			//Act
			var result = await controller.DeletePost(ConstIds.ExamplePostId);

			//Act
			var noContentResult = Assert.IsType<NoContentResult>(result);
			_mockPostService.Verify();
		}
		[Fact]
		public async void DeletePost_ReturnsBadRequestObjectResult_WhenThePostGuidIdIsInvalid()
		{
			//Arrange
			var controller = new PostsController(_loggerMock.Object, _mockPostService.Object, _mockUserService.Object);
			//Act
			var result = await controller.DeletePost(ConstIds.InvalidGuid);

			//Assert
			var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal($"{ConstIds.InvalidGuid} is not valid guid.", badRequestObjectResult.Value);
		}
		[Fact]
		public async void DeletePost_ReturnsInternalServerErrorResult_WhenExceptionThrownInService()
		{
			//Arrange
			_mockPostService.Setup(Service => Service.DeletePostAsync(It.IsAny<Guid>()))
				.Throws(new ArgumentNullException(nameof(Guid)))
				.Verifiable();

			var controller = new PostsController(_loggerMock.Object, _mockPostService.Object, _mockUserService.Object);
			//Act
			var result = await controller.DeletePost(ConstIds.ExamplePostId);

			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockPostService.Verify();
		}
	}
}
