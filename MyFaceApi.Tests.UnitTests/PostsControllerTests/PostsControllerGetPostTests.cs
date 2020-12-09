using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyFaceApi.Api.Controllers;
using MyFaceApi.Api.DataAccess.Entities;
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
		public void GetPost_ReturnsOkObjectResult_WithPostData()
		{
			//Arrange
			var post = GetTestUserData().ElementAt(0).Posts.ElementAt(0);

			_mockPostRepo.Setup(repo => repo.GetPost(It.IsAny<Guid>()))
				.Returns(post)
				.Verifiable();

			var controller = new PostsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper, _mockRelationsRepo.Object, _mockImageManager.Object);

			//Act
			var result = controller.GetPost(ConstIds.ExamplePostId);

			//Assert
			var actionResult = Assert.IsType<OkObjectResult>(result.Result);
			var model = Assert.IsType<Post>(actionResult.Value);
			Assert.Equal(post, model);
			_mockPostRepo.Verify();
		}
		[Fact]
		public void GetPost_ReturnsBadRequestObjectResult_WhenThePostIdIsInvalid()
		{
			//Arrange
			var controller = new PostsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper, _mockRelationsRepo.Object, _mockImageManager.Object);

			//Act
			var result = controller.GetPost(ConstIds.InvalidGuid);

			//Assert
			var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result.Result);
			Assert.Equal($"{ConstIds.InvalidGuid} is not valid guid.", badRequestObjectResult.Value);
		}
		[Fact]
		public void GetPost_ReturnsNotFoundResult_WhenThePostDoesntExist()
		{
			//Arrange
			_mockPostRepo.Setup(repo => repo.GetPost(It.IsAny<Guid>()))
				.Returns((Post)null)
				.Verifiable();

			var controller = new PostsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper, _mockRelationsRepo.Object, _mockImageManager.Object);

			//Act
			var result = controller.GetPost(ConstIds.ExamplePostId);

			//Assert
			var notFoundResult = Assert.IsType<NotFoundResult>(result.Result);
			_mockPostRepo.Verify();
		}
		[Fact]
		public void GetPost_ReturnsInternalServerErrorResult_WhenExceptionThrownInRepository()
		{
			//Arrange
			_mockPostRepo.Setup(repo => repo.GetPost(It.IsAny<Guid>()))
				.Throws(new ArgumentNullException(nameof(Guid)))
				.Verifiable();

			var controller = new PostsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper, _mockRelationsRepo.Object, _mockImageManager.Object);

			//Act
			var result = controller.GetPost(ConstIds.ExamplePostId);
			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result.Result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockPostRepo.Verify();
		}
	}
}
