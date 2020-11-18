using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyFaceApi.Controllers;
using MyFaceApi.Entities;
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
		public void GetPost_ReturnsAnActionResult_WithPost()
		{
			//Arrange
			var user = GetTestUserData().ElementAt(0);

			_mockPostRepo.Setup(repo => repo.GetPost(It.IsAny<Guid>()))
				.Returns(user.Posts.ElementAt(0))
				.Verifiable();

			var controller = new PostsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper);

			//Act
			var result = controller.GetPost(user.Posts.ElementAt(0).Id.ToString());

			//Assert
			var actionResult = Assert.IsType<OkObjectResult>(result.Result);
			var model = Assert.IsType<Post>(actionResult.Value);
			Assert.Equal(user.Posts.ElementAt(0), model);
			_mockPostRepo.Verify();
		}
		[Fact]
		public void GetPost_ReturnsBadRequestResult_WhenTheUserIdIsInvalid()
		{
			//Arrange
			var controller = new PostsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper);

			//Act
			var result = controller.GetPost("InvalidGuid");

			//Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
			Assert.Equal("InvalidGuid is not valid Guid.", badRequestResult.Value);
		}
		[Fact]
		public void GetPost_ReturnsNotFoundObjectResult_WhenTheUserDoesntExist()
		{
			//Arrange
			_mockPostRepo.Setup(repo => repo.GetPost(It.IsAny<Guid>()))
				.Returns((Post)null)
				.Verifiable();

			var controller = new PostsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper);

			//Act
			var result = controller.GetPost(_exaplePostGuid);

			//Assert
			var notFoundResult = Assert.IsType<NotFoundResult>(result.Result);
			_mockPostRepo.Verify();
		}
		[Fact]
		public void GetPost_ReturnsInternalServerError_WhenExceptionThrownInRepository()
		{
			//Arrange
			_mockPostRepo.Setup(repo => repo.GetPost(It.IsAny<Guid>()))
				.Throws(new ArgumentNullException(nameof(_exaplePostGuid)))
				.Verifiable();

			var controller = new PostsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper);

			//Act
			var result = controller.GetPost(_exaplePostGuid);
			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result.Result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockUserRepo.Verify();
			_mockPostRepo.Verify();
		}
	}
}
