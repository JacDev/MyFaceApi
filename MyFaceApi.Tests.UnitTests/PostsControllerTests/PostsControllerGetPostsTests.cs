using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyFaceApi.Api.Application.DtoModels.Post;
using MyFaceApi.Api.Controllers;
using Pagination.Helpers;
using System;
using System.Collections.Generic;
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
			var posts = GetTestPostData();
			var pagedList = PagedList<PostDto>.Create(posts, _paginationsParams.PageNumber, _paginationsParams.PageSize, _paginationsParams.Skip);
			_mockUserService.Setup(Service => Service.CheckIfUserExists(It.IsAny<Guid>()))
				.ReturnsAsync(true)
				.Verifiable();
			_mockPostService.Setup(Service => Service.GetUserPosts(It.IsAny<Guid>(), _paginationsParams))
				.Returns(pagedList)
				.Verifiable();

			var controller = new PostsController(_loggerMock.Object, _mockPostService.Object, _mockUserService.Object);

			//Act
			var result = controller.GetPosts(ConstIds.ExampleUserId, _paginationsParams);

			//Assert
			var actionResult = Assert.IsType<OkObjectResult>(result.Result);
			var model = Assert.IsType<List<PostDto>>(actionResult.Value);

			Assert.Equal(posts.Count, model.Count);
			_mockUserService.Verify();
			_mockPostService.Verify();
		}
		[Fact]
		public async void GetPosts_ReturnsBadRequestObjectResult_WhenTheUserIdIsInvalid()
		{
			//Arrange
			var controller = new PostsController(_loggerMock.Object, _mockPostService.Object, _mockUserService.Object);

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
			_mockUserService.Setup(Service => Service.CheckIfUserExists(It.IsAny<Guid>()))
				.ReturnsAsync(false)
				.Verifiable();

			var controller = new PostsController(_loggerMock.Object, _mockPostService.Object, _mockUserService.Object);

			//Act
			var result = await controller.GetPosts(ConstIds.ExampleUserId, _paginationsParams);

			//Assert
			var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result.Result);
			Assert.Equal($"User: {ConstIds.ExampleUserId} not found.", notFoundObjectResult.Value);
			_mockUserService.Verify();
		}
		[Fact]
		public async void GetPosts_ReturnsInternalServerErrorResult_WhenExceptionThrownInService()
		{
			//Arrange
			_mockUserService.Setup(Service => Service.CheckIfUserExists(It.IsAny<Guid>()))
				.Throws(new ArgumentNullException(nameof(Guid)))
				.Verifiable();

			var controller = new PostsController(_loggerMock.Object, _mockPostService.Object, _mockUserService.Object);

			//Act
			var result = await controller.GetPosts(ConstIds.ExampleUserId, _paginationsParams);
			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result.Result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockUserService.Verify();
		}
	}
}
