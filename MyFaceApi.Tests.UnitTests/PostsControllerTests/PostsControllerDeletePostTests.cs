using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyFaceApi.Controllers;
using MyFaceApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace MyFaceApi.Tests.UnitTests.PostsControllerTests
{
	public class PostsControllerDeletePostTests : PostsControllerPreparation
	{
		public PostsControllerDeletePostTests() : base()
		{
		}
		[Fact]
		public async void DeletePost_ReturnsNoContentResult_WhenTheUserHasBeenRemoved()
		{
			//Arrange
			var postToRemove = GetTestUserData().ElementAt(0).Posts.ElementAt(0);
			_mockPostRepo.Setup(repo => repo.GetPost(It.IsAny<Guid>()))
				.Returns(postToRemove)
				.Verifiable();
			_mockPostRepo.Setup(repo => repo.DeletePostAsync(It.IsAny<Post>()))
				.Verifiable();

			var controller = new PostsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper);

			//Act
			var result = await controller.DeletePost(postToRemove.Id.ToString());

			//Act
			var noContentResult = Assert.IsType<NoContentResult>(result);
			_mockPostRepo.Verify();
		}
		[Fact]
		public async void DeletePost_ReturnsBadRequestResult_WhenTheUserGuidIdIsInvalid()
		{
			//Arrange
			var controller = new PostsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper);

			//Act
			var result = await controller.DeletePost("InvalidGuid");

			//Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal("InvalidGuid is not valid Guid.", badRequestResult.Value);
		}
		[Fact]
		public async void DeletePost_ReturnsNotFoundResult_WhenTheUserDoesntExist()
		{
			//Arrange
			_mockPostRepo.Setup(repo => repo.GetPost(It.IsAny<Guid>()))
				.Returns((Post)null)
				.Verifiable();

			var controller = new PostsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper);

			//Act
			var result = await controller.DeletePost(_exaplePostGuid);

			//Assert
			var notFoundResult = Assert.IsType<NotFoundResult>(result);
			_mockPostRepo.Verify();
		}
		[Fact]
		public async void DeletePost_ReturnsInternalServerError_WhenExceptionThrownInRepository()
		{
			//Arrange
			_mockPostRepo.Setup(repo => repo.GetPost(It.IsAny<Guid>()))
				.Throws(new ArgumentNullException(nameof(_exaplePostGuid)))
				.Verifiable();

			var controller = new PostsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper);

			//Act
			var result = await controller.DeletePost(_exaplePostGuid);

			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockPostRepo.Verify();
		}
	}
}
