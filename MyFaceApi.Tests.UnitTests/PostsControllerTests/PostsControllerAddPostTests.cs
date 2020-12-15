using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyFaceApi.Api.Controllers;
using System;
using Xunit;
using AutoFixture;
using MyFaceApi.Api.Application.DtoModels.Post;

namespace MyFaceApi.Tests.UnitTests.PostsControllerTests
{
	public class PostsControllerAddPostTests : PostsControllerPreparation
	{
		protected readonly PostToAddDto _postToAdd;
		public PostsControllerAddPostTests() : base()
		{
			_postToAdd = _fixture.Create<PostToAddDto>();
		}
		[Fact]
		public async void AddPost_ReturnsCreatedAtRouteResult_WithPostData()
		{
			//Arrange
			_mockUserService.Setup(Service => Service.CheckIfUserExists(It.IsAny<Guid>()))
				.ReturnsAsync(true)
				.Verifiable();

			var postEntity = _mapper.Map<PostDto>(_postToAdd);

			_mockPostService.Setup(Service => Service.AddPostAsync(Guid.Parse(ConstIds.ExampleUserId), It.IsAny<PostToAddDto>()))
				.ReturnsAsync(postEntity)
				.Verifiable();
			var controller = new PostsController(_loggerMock.Object, _mockPostService.Object, _mockUserService.Object);

			//Act
			var result = await controller.AddPost(ConstIds.ExampleUserId, _postToAdd);

			//Assert
			var redirectToActionResult = Assert.IsType<CreatedAtRouteResult>(result.Result);
			Assert.Equal(ConstIds.ExampleUserId, redirectToActionResult.RouteValues["userId"].ToString());
			Assert.Equal(Guid.Empty.ToString(), redirectToActionResult.RouteValues["postId"].ToString());
			Assert.Equal("GetPost", redirectToActionResult.RouteName);
			Assert.IsType<PostDto>(redirectToActionResult.Value);

			_mockUserService.Verify();
			_mockPostService.Verify();
		}
		[Fact]
		public async void AddPost_ReturnsBadRequestObjectResult_WhenTheUserIdIsInvalid()
		{
			//Arrange
			var controller = new PostsController(_loggerMock.Object, _mockPostService.Object, _mockUserService.Object);

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
			_mockUserService.Setup(Service => Service.CheckIfUserExists(It.IsAny<Guid>()))
				.ReturnsAsync(false)
				.Verifiable();

			var controller = new PostsController(_loggerMock.Object, _mockPostService.Object, _mockUserService.Object);

			//Act
			var result = await controller.AddPost(ConstIds.ExampleUserId, _postToAdd);

			//Assert
			var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result.Result);
			Assert.Equal($"User: {ConstIds.ExampleUserId} not found.", notFoundObjectResult.Value);
			_mockUserService.Verify();
		}
		[Fact]
		public async void AddPost_ReturnsInternalServerErrorResult_WhenExceptionThrownInService()
		{
			//Arrange
			_mockUserService.Setup(Service => Service.CheckIfUserExists(It.IsAny<Guid>()))
				.Throws(new ArgumentNullException(nameof(Guid)))
				.Verifiable();

			var controller = new PostsController(_loggerMock.Object, _mockPostService.Object, _mockUserService.Object);

			//Act
			var result = await controller.AddPost(ConstIds.ExampleUserId, _postToAdd);
			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result.Result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockUserService.Verify();
		}
	}
}
