using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyFaceApi.Controllers;
using MyFaceApi.Entities;
using MyFaceApi.Models.PostModels;
using System;
using System.Linq;
using Xunit;

namespace MyFaceApi.Tests.UnitTests.PostsControllerTests
{
	public class PostsControllerAddPostTests : PostsControllerPreparation
	{
		public PostsControllerAddPostTests() : base()
		{
		}
		[Fact]
		public async void AddPost_ReturnsRedirectToAddedUser_WithPostData()
		{
			//Arrange
			var testUser = GetTestUserData().ElementAt(0);
			var postToAdd = new PostToAdd
			{
				WhenAdded = DateTime.Now,
				Text = "Added in test"
			};
			_mockUserRepo.Setup(repo => repo.CheckIfUserExists(It.IsAny<Guid>()))
				.Returns(true)
				.Verifiable();
			Post postEntity = _mapper.Map<Post>(postToAdd);
			postEntity.Id = new Guid(_exaplePostGuid);

			_mockPostRepo.Setup(repo => repo.AddPostAsync(It.IsAny<Post>()))
				.ReturnsAsync(postEntity)
				.Verifiable();

			var controller = new PostsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper);

			//Act
			var result = await controller.AddPost(testUser.Id.ToString(), postToAdd);

			//Assert
			var redirectToActionResult = Assert.IsType<CreatedAtRouteResult>(result.Result);
			Assert.Equal(testUser.Id.ToString(), redirectToActionResult.RouteValues["userId"].ToString());
			Assert.Equal(postEntity.Id.ToString(), redirectToActionResult.RouteValues["postId"].ToString());
			Assert.Equal("GetPost", redirectToActionResult.RouteName);
			_mockUserRepo.Verify();
		}
		[Fact]
		public async void AddPost_ReturnsBadRequestResult_WhenTheUserIdIsInvalid()
		{
			//Arrange
			var controller = new PostsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper);

			//Act
			var result = await controller.AddPost("InvalidGuid", new PostToAdd { Text = "Bad request" });

			//Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
			Assert.Equal("InvalidGuid is not valid Guid.", badRequestResult.Value);
		}
		[Fact]
		public async void AddPost_ReturnsNotFoundObjectResult_WhenTheUserDoesntExist()
		{
			//Arrange
			_mockUserRepo.Setup(repo => repo.CheckIfUserExists(It.IsAny<Guid>()))
				.Returns(false)
				.Verifiable();

			var controller = new PostsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper);

			//Act
			var result = await controller.AddPost(_exaplePostGuid, new PostToAdd { Text = "Bad request" });

			//Assert
			var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
			Assert.Equal($"User: {_exaplePostGuid} not found.", notFoundResult.Value);
			_mockUserRepo.Verify();
			_mockPostRepo.Verify();
		}
		[Fact]
		public async void AddPost_ReturnsInternalServerError_WhenExceptionThrownInRepository()
		{
			//Arrange
			_mockUserRepo.Setup(repo => repo.CheckIfUserExists(It.IsAny<Guid>()))
				.Returns(true)
				.Verifiable();
			_mockPostRepo.Setup(repo => repo.AddPostAsync(It.IsAny<Post>()))
				.Throws(new ArgumentNullException(nameof(_exaplePostGuid)))
				.Verifiable();

			var controller = new PostsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper);

			//Act
			var result = await controller.AddPost(_exaplePostGuid, new PostToAdd { Text = "Bad request" });
			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result.Result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockUserRepo.Verify();
			_mockPostRepo.Verify();
		}
	}
}
