using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyFaceApi.Controllers;
using MyFaceApi.DataAccess.Entities;
using MyFaceApi.Models.PostModels;
using System;
using Xunit;
using AutoFixture;

namespace MyFaceApi.Tests.UnitTests.PostsControllerTests
{
	public class PostsControllerAddPostTests : PostsControllerPreparation
	{
		protected readonly PostToAdd _postToAdd;
		public PostsControllerAddPostTests() : base()
		{
			_postToAdd = _fixture.Create<PostToAdd>();
		}
		[Fact]
		public async void AddPost_ReturnsCreatedAtRouteResult_WithPostData()
		{
			//Arrange
			_mockUserRepo.Setup(repo => repo.CheckIfUserExists(It.IsAny<Guid>()))
				.Returns(true)
				.Verifiable();
			Post postEntity = _mapper.Map<Post>(_postToAdd);
			postEntity.Id = new Guid(ConstIds.ExamplePostId);

			_mockPostRepo.Setup(repo => repo.AddPostAsync(It.IsAny<Post>()))
				.ReturnsAsync(postEntity)
				.Verifiable();

			var controller = new PostsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper);

			//Act
			var result = await controller.AddPost(ConstIds.ExampleUserId, _postToAdd);

			//Assert
			var redirectToActionResult = Assert.IsType<CreatedAtRouteResult>(result.Result);
			Assert.Equal(ConstIds.ExampleUserId, redirectToActionResult.RouteValues["userId"].ToString());
			Assert.Equal(ConstIds.ExamplePostId, redirectToActionResult.RouteValues["postId"].ToString());
			Assert.Equal("GetPost", redirectToActionResult.RouteName);
			Assert.IsType<Post>(redirectToActionResult.Value);

			_mockUserRepo.Verify();
			_mockPostRepo.Verify();
		}
		[Fact]
		public async void AddPost_ReturnsBadRequestObjectResult_WhenTheUserIdIsInvalid()
		{
			//Arrange
			var controller = new PostsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper);

			//Act
			var result = await controller.AddPost(ConstIds.InvalidGuid, _postToAdd);

			//Assert
			var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result.Result);
			Assert.Equal($"{ConstIds.InvalidGuid} is not valid guid.", badRequestObjectResult.Value);
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
			var result = await controller.AddPost(ConstIds.ExampleUserId, _postToAdd);

			//Assert
			var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result.Result);
			Assert.Equal($"User: {ConstIds.ExampleUserId} not found.", notFoundObjectResult.Value);
			_mockUserRepo.Verify();
		}
		[Fact]
		public async void AddPost_ReturnsInternalServerErrorResult_WhenExceptionThrownInRepository()
		{
			//Arrange
			_mockUserRepo.Setup(repo => repo.CheckIfUserExists(It.IsAny<Guid>()))
				.Throws(new ArgumentNullException(nameof(Guid)))
				.Verifiable();

			var controller = new PostsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper);

			//Act
			var result = await controller.AddPost(ConstIds.ExampleUserId, _postToAdd);
			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result.Result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockUserRepo.Verify();
		}
	}
}
