using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyFaceApi.Api.Application.DtoModels.PostComment;
using MyFaceApi.Api.Controllers;
using Pagination.Helpers;
using System;
using Xunit;

namespace MyFaceApi.Api.Tests.UnitTests.PostCommentsControllerTests
{
	public class PostCommentsControllerGetCommentsTests : PostCommentsControllerPreparation
	{
		public PostCommentsControllerGetCommentsTests() : base()
		{
		}
		[Fact]
		public void GetComments_ReturnsOkObjectResult_WithListOfCommentsData()
		{
			//Arrange
			var comments = GetTestPostData();
			var pagedList = PagedList<PostCommentDto>.Create(comments, _paginationsParams.PageNumber, _paginationsParams.PageSize, _paginationsParams.Skip);
			_mockPostService.Setup(Service => Service.CheckIfPostExists(It.IsAny<Guid>()))
				.Returns(true)
				.Verifiable();
			_mockCommentService.Setup(Service => Service.GetPostComments(It.IsAny<Guid>(), _paginationsParams))
				.Returns(pagedList)
				.Verifiable();

			var controller = new PostCommentsController(_loggerMock.Object, _mockPostService.Object, _mockUserService.Object, _mockCommentService.Object);

			//Act
			var result = controller.GetComments(ConstIds.ExamplePostId, _paginationsParams);

			//Assert
			var actionResult = Assert.IsType<OkObjectResult>(result.Result);
			var model = Assert.IsType<PagedList<PostCommentDto>>(actionResult.Value);
			_mockCommentService.Verify();
			_mockPostService.Verify();
		}
		[Fact]
		public void GetComments_ReturnsBadRequestObjectResult_WhenThePostIdIsInvalid()
		{
			//Arrange
			var controller = new PostCommentsController(_loggerMock.Object, _mockPostService.Object, _mockUserService.Object, _mockCommentService.Object);

			//Act
			var result = controller.GetComments(ConstIds.InvalidGuid, _paginationsParams);

			//Assert
			var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result.Result);
			Assert.Equal($"{ConstIds.InvalidGuid} is not valid guid.", badRequestObjectResult.Value);
		}
		[Fact]
		public void GetComments_ReturnsNotFoundObjectResult_WhenThePostDoesntExist()
		{
			//Arrange
			_mockPostService.Setup(Service => Service.CheckIfPostExists(It.IsAny<Guid>()))
				.Returns(false)
				.Verifiable();

			var controller = new PostCommentsController(_loggerMock.Object, _mockPostService.Object, _mockUserService.Object, _mockCommentService.Object);

			//Act
			var result = controller.GetComments(ConstIds.ExamplePostId, _paginationsParams);

			//Assert
			var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result.Result);
			Assert.Equal($"Post: {ConstIds.ExamplePostId} not found.", notFoundObjectResult.Value);
			_mockPostService.Verify();
		}
		[Fact]
		public void GetComments_ReturnsInternalServerErrorResult_WhenExceptionThrownInService()
		{
			//Arrange
			_mockPostService.Setup(Service => Service.CheckIfPostExists(It.IsAny<Guid>()))
				.Throws(new ArgumentNullException(nameof(Guid)))
				.Verifiable();

			var controller = new PostCommentsController(_loggerMock.Object, _mockPostService.Object, _mockUserService.Object, _mockCommentService.Object);

			//Act
			var result = controller.GetComments(ConstIds.ExamplePostId, _paginationsParams);

			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result.Result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockPostService.Verify();
		}
	}
}
