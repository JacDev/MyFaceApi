using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyFaceApi.Api.Controllers;
using System;
using System.Threading.Tasks;
using Xunit;

namespace MyFaceApi.Tests.UnitTests.PostsControllerTests
{
	public class PostsControllerPatchPostTests : PostsControllerPreparation
	{
		public PostsControllerPatchPostTests() : base()
		{
		}
		[Fact]
		public async void PartiallyUpdatePost_ReturnsNoContentResult_WhenThePostHasBeenUpdated()
		{
			//Arrange
			_mockPostService.Setup(Service => Service.TryUpdatePostAsync(It.IsAny<Guid>(), GetJsonPatchDocument()))
				.ReturnsAsync(true)
				.Verifiable();

			var controller = new PostsController(_loggerMock.Object, _mockPostService.Object, _mockUserService.Object);

			//Act
			var result = await controller.PartiallyUpdatePost(ConstIds.ExamplePostId, GetJsonPatchDocument());

			//Assert
			var noContentResult = Assert.IsType<NoContentResult>(result);
			_mockPostService.Verify();
		}
		[Fact]
		public async void PartiallyUpdatePost_ReturnsBadRequestResult_WhenThePostHasNotBeenUpdated()
		{
			//Arrange
			_mockPostService.Setup(Service => Service.TryUpdatePostAsync(It.IsAny<Guid>(), GetJsonPatchDocument()))
				.ReturnsAsync(false)
				.Verifiable();

			var controller = new PostsController(_loggerMock.Object, _mockPostService.Object, _mockUserService.Object);

			//Act
			var result = await controller.PartiallyUpdatePost(ConstIds.ExamplePostId, GetJsonPatchDocument());

			//Assert
			var noContentResult = Assert.IsType<BadRequestResult>(result);
			_mockPostService.Verify();
		}
		[Fact]
		public async void PartiallyUpdatePost_ReturnsBadRequestObjectResult_WhenThePostIdIsInvalid()
		{
			//Arrange
			var controller = new PostsController(_loggerMock.Object, _mockPostService.Object, _mockUserService.Object);

			//Act
			var result = await controller.PartiallyUpdatePost(ConstIds.InvalidGuid, GetJsonPatchDocument());

			//Assert
			var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal($"{ConstIds.InvalidGuid} is not valid guid.", badRequestObjectResult.Value);
		}
		[Fact]
		public async void PartiallyUpdatePost_ReturnsInternalServerErrorResult_WhenExceptionThrownInService()
		{
			//Arrange
			_mockPostService.Setup(Service => Service.TryUpdatePostAsync(It.IsAny<Guid>(), GetJsonPatchDocument()))
				.ThrowsAsync(new ArgumentNullException(nameof(Guid)))
				.Verifiable();

			var controller = new PostsController(_loggerMock.Object, _mockPostService.Object, _mockUserService.Object);

			//Act
			var result = await controller.PartiallyUpdatePost(ConstIds.ExamplePostId, GetJsonPatchDocument());

			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockPostService.Verify();
		}
	}
}
