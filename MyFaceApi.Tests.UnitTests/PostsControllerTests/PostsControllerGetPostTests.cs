using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyFaceApi.Api.Application.DtoModels.Post;
using MyFaceApi.Api.Controllers;
using System;
using System.Linq;
using Xunit;

namespace MyFaceApi.Tests.UnitTests.PostsControllerTests
{
	public class PostsControllerGetPostTests : PostsControllerPreparation
	{
		public PostsControllerGetPostTests() : base()
		{
		}
		[Fact]
		public void GetPost_ReturnsOkObjectResult_WithPostDto()
		{
			//Arrange
			var post = GetTestPostData().ElementAt(0);

			_mockPostService.Setup(Service => Service.GetPost(It.IsAny<Guid>()))
				.Returns(post)
				.Verifiable();

			var controller = new PostsController(_loggerMock.Object, _mockPostService.Object, _mockUserService.Object);

			//Act
			var result = controller.GetPost(ConstIds.ExamplePostId);

			//Assert
			var actionResult = Assert.IsType<OkObjectResult>(result.Result);
			var model = Assert.IsType<PostDto>(actionResult.Value);
			Assert.Equal(post, model);
			_mockPostService.Verify();
		}
		[Fact]
		public void GetPost_ReturnsBadRequestObjectResult_WhenThePostIdIsInvalid()
		{
			//Arrange
			var controller = new PostsController(_loggerMock.Object, _mockPostService.Object, _mockUserService.Object);

			//Act
			var result = controller.GetPost(ConstIds.InvalidGuid);

			//Assert
			var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result.Result);
			Assert.Equal($"{ConstIds.InvalidGuid} is not valid guid.", badRequestObjectResult.Value);
		}
		[Fact]
		public void GetPost_ReturnsOkObjectResult_WithNullPostDto()
		{
			//Arrange
			_mockPostService.Setup(Service => Service.GetPost(It.IsAny<Guid>()))
				.Returns((PostDto)null)
				.Verifiable();

			var controller = new PostsController(_loggerMock.Object, _mockPostService.Object, _mockUserService.Object);

			//Act
			var result = controller.GetPost(ConstIds.ExamplePostId);

			//Assert
			var actionResult = Assert.IsType<OkObjectResult>(result.Result);
			var model = Assert.IsType<PostDto>(actionResult.Value);
			Assert.Null(model);
			_mockPostService.Verify();
		}
		[Fact]
		public void GetPost_ReturnsInternalServerErrorResult_WhenExceptionThrownInService()
		{
			//Arrange
			_mockPostService.Setup(Service => Service.GetPost(It.IsAny<Guid>()))
				.Throws(new ArgumentNullException(nameof(Guid)))
				.Verifiable();

			var controller = new PostsController(_loggerMock.Object, _mockPostService.Object, _mockUserService.Object);

			//Act
			var result = controller.GetPost(ConstIds.ExamplePostId);
			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result.Result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockPostService.Verify();
		}
	}
}
