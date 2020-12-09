using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyFaceApi.Api.Controllers;
using MyFaceApi.Api.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MyFaceApi.Tests.UnitTests.PostsControllerTests
{
	public class PostsControllerGetPostsTests : PostsControllerPreparation
	{
		public PostsControllerGetPostsTests() : base()
		{
		}
		[Fact]
		public void GetPosts_ReturnsOkObjectResult_WithAListOfPostsData()
		{
			//Arrange
			var user = GetTestUserData().ElementAt(0);

			_mockUserRepo.Setup(repo => repo.CheckIfUserExists(It.IsAny<Guid>()))
				.ReturnsAsync(true)
				.Verifiable();
			_mockPostRepo.Setup(repo => repo.GetUserPosts(It.IsAny<Guid>()))
				.Returns(user.Posts.ToList())
				.Verifiable();

			var controller = new PostsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper, _mockRelationsRepo.Object, _mockImageManager.Object);

			//Act
			var result = controller.GetPosts(ConstIds.ExampleUserId, _paginationsParams);

			//Assert
			var actionResult = Assert.IsType<OkObjectResult>(result.Result);
			var model = Assert.IsType<List<Post>>(actionResult.Value);

			Assert.Equal(user.Posts.Count, model.Count);
			_mockUserRepo.Verify();
			_mockPostRepo.Verify();
		}
		[Fact]
		public async void GetPosts_ReturnsBadRequestObjectResult_WhenTheUserIdIsInvalid()
		{
			//Arrange
			var controller = new PostsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper, _mockRelationsRepo.Object, _mockImageManager.Object);

			//Act
			var result = await controller.GetPosts(ConstIds.InvalidGuid, _paginationsParams);

			//Assert
			var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result.Result);
			Assert.Equal($"{ConstIds.InvalidGuid} is not valid guid.", badRequestObjectResult.Value);
		}
		[Fact]
		public async void GetPosts_ReturnsNotFoundObjectResult_WhenTheUserDoesntExist()
		{
			//Arrange
			_mockUserRepo.Setup(repo => repo.CheckIfUserExists(It.IsAny<Guid>()))
				.ReturnsAsync(false)
				.Verifiable();

			var controller = new PostsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper, _mockRelationsRepo.Object, _mockImageManager.Object);

			//Act
			var result = await controller.GetPosts(ConstIds.ExampleUserId, _paginationsParams);

			//Assert
			var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result.Result);
			Assert.Equal($"User: {ConstIds.ExampleUserId} not found.", notFoundObjectResult.Value);
			_mockUserRepo.Verify();
		}
		[Fact]
		public async void GetPosts_ReturnsInternalServerErrorResult_WhenExceptionThrownInRepository()
		{
			//Arrange
			_mockUserRepo.Setup(repo => repo.CheckIfUserExists(It.IsAny<Guid>()))
				.Throws(new ArgumentNullException(nameof(Guid)))
				.Verifiable();

			var controller = new PostsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper, _mockRelationsRepo.Object, _mockImageManager.Object);

			//Act
			var result = await controller.GetPosts(ConstIds.ExampleUserId, _paginationsParams);
			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result.Result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockUserRepo.Verify();
		}
	}
}
