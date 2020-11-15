using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyFaceApi.Controllers;
using MyFaceApi.Entities;
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
		public void GetPosts_ReturnsAnActionResult_WithAListOfPosts()
		{
			//Arrange
			var user = GetTestUserData().ElementAt(0);

			_mockUserRepo.Setup(repo => repo.CheckIfUserExists(user.Id))
				.Returns(true);
			_mockPostRepo.Setup(repo => repo.GetUserPosts(user.Id))
				.Returns(user.Posts.ToList());

			var controller = new PostsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper);

			//Act
			var result = controller.GetPosts(user.Id.ToString());

			//Assert
			var actionResult = Assert.IsType<OkObjectResult>(result.Result);
			var model = Assert.IsType<List<Post>>(actionResult.Value);

			Assert.Equal(user.Posts.Count, model.Count);
			_mockUserRepo.Verify();
			_mockPostRepo.Verify();
		}
		[Fact]
		public void GetPosts_ReturnsBadRequestResult_WhenTheUserIdIsInvalid()
		{
			//Arrange
			var controller = new PostsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper);

			//Act
			var result = controller.GetPosts("InvalidGuid");

			//Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
			Assert.Equal("InvalidGuid is not valid Guid.", badRequestResult.Value);
		}
		[Fact]
		public void GetPosts_ReturnsNotFoundObjectResult_WhenTheUserDoesntExist()
		{
			//Arrange
			Guid testUserGuid = new Guid("24610263-CEE4-4231-97DF-904EE6437278");

			_mockUserRepo.Setup(repo => repo.CheckIfUserExists(testUserGuid))
				.Returns(false);

			var controller = new PostsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper);

			//Act
			var result = controller.GetPosts(testUserGuid.ToString());

			//Assert
			var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
			Assert.Equal($"User: {testUserGuid} not found.", notFoundResult.Value);
			_mockUserRepo.Verify();
			_mockPostRepo.Verify();
		}
		[Fact]
		public void GetUser_ReturnsInternalServerError_WhenExceptionThrownInRepository()
		{
			//Arrange
			Guid testUserGuid = new Guid("24610263-CEE4-4231-97DF-904EE6437278");

			_mockUserRepo.Setup(repo => repo.CheckIfUserExists(testUserGuid))
				.Returns(true);
			_mockPostRepo.Setup(repo => repo.GetUserPosts(testUserGuid))
				.Throws(new ArgumentNullException(nameof(testUserGuid)));


			var controller = new PostsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper);

			//Act
			var result = controller.GetPosts(testUserGuid.ToString());
			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result.Result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockUserRepo.Verify();
			_mockPostRepo.Verify();
		}
	}
}
