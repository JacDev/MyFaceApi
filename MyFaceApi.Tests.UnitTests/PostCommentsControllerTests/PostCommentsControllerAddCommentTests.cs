using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyFaceApi.Api.Controllers;
using System;
using Xunit;
using AutoFixture;
using MyFaceApi.Api.Application.DtoModels.PostComment;

namespace MyFaceApi.Tests.UnitTests.PostCommentsControllerTests
{
	public class PostCommentsControllerAddCommentTests : PostCommentsControllerPreparation
	{
		protected readonly PostCommentToAddDto _commentToAdd;
		public PostCommentsControllerAddCommentTests() : base()
		{
			_commentToAdd = _fixture.Create<PostCommentToAddDto>();
		}
		[Fact]
		public async void AddComment_ReturnsCreatedAtRouteResult_WithPostCommentData()
		{
			//Arrange
			_mockPostService.Setup(Service => Service.CheckIfPostExists(It.IsAny<Guid>()))
				.Returns(true)
				.Verifiable();
			_mockUserService.Setup(Service => Service.CheckIfUserExists(It.IsAny<Guid>()))
				.ReturnsAsync(true)
				.Verifiable();

			var commentEntity = _mapper.Map<PostCommentDto>(_commentToAdd);
			commentEntity.Id = new Guid(ConstIds.ExampleCommentId);

			_mockCommentService.Setup(Service => Service.AddCommentAsync(It.IsAny<Guid>(), It.IsAny<PostCommentToAddDto>()))
				.ReturnsAsync(commentEntity)
				.Verifiable();

			var controller = new PostCommentsController(_loggerMock.Object, _mockPostService.Object, _mockUserService.Object, _mockCommentService.Object);

			//Act
			var result = await controller.AddComment(ConstIds.ExampleUserId, ConstIds.ExamplePostId, _commentToAdd);

			//Assert
			var redirectToActionResult = Assert.IsType<CreatedAtRouteResult>(result.Result);
			Assert.Equal(ConstIds.ExampleUserId, redirectToActionResult.RouteValues["userId"].ToString());
			//Assert.Equal(_commentToAdd.PostId.ToString(), redirectToActionResult.RouteValues["postId"].ToString());
			Assert.Equal(ConstIds.ExampleCommentId, redirectToActionResult.RouteValues["commentId"].ToString());
			Assert.Equal("GetComment", redirectToActionResult.RouteName);
			_mockCommentService.Verify();
			_mockPostService.Verify();
			_mockUserService.Verify();
		}
		[Theory]
		[InlineData(ConstIds.ExampleUserId, ConstIds.InvalidGuid)]
		[InlineData(ConstIds.InvalidGuid, ConstIds.ExamplePostId)]
		public async void AddComment_ReturnsBadRequestObjectResult_WhenThePostOrUserIdIsInvalid(string userTestId, string postTestId)
		{
			//Arrange
			var controller = new PostCommentsController(_loggerMock.Object, _mockPostService.Object, _mockUserService.Object, _mockCommentService.Object);

			//Act
			var result = await controller.AddComment(userTestId, postTestId, _commentToAdd);

			//Assert
			var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result.Result);
			Assert.Equal($"{userTestId} or {postTestId} is not valid guid.", badRequestObjectResult.Value);
		}
		[Theory]
		[InlineData(true, false)]
		[InlineData(false, true)]
		public async void AddComment_ReturnsNotFoundObjectResult_WhenThePostOrUserDoesntExist(bool doesTheUserExists, bool doesThePostExists)
		{
			//Arrange
			_mockPostService.Setup(Service => Service.CheckIfPostExists(It.IsAny<Guid>()))
				.Returns(doesThePostExists)
				.Verifiable();
			_mockUserService.Setup(Service => Service.CheckIfUserExists(It.IsAny<Guid>()))
				.ReturnsAsync(doesTheUserExists)
				.Verifiable();
			var controller = new PostCommentsController(_loggerMock.Object, _mockPostService.Object, _mockUserService.Object, _mockCommentService.Object);

			//Act
			var result = await controller.AddComment(ConstIds.ExampleUserId, ConstIds.ExamplePostId, _commentToAdd);

			//Assert
			var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result.Result);
			Assert.Equal($"User: {ConstIds.ExampleUserId} or post {ConstIds.ExamplePostId} not found.", notFoundObjectResult.Value);
			_mockPostService.Verify();
			//if post doesnt exist the user will not be checked
			if (doesThePostExists)
			{
				_mockUserService.Verify();
			}
		}
		[Fact]
		public async void AddComment_ReturnsInternalServerErrorResult_WhenExceptionThrownInService()
		{
			//Arrange
			_mockPostService.Setup(Service => Service.CheckIfPostExists(It.IsAny<Guid>()))
				.Throws(new ArgumentNullException(nameof(Guid)))
				.Verifiable();

			var controller = new PostCommentsController(_loggerMock.Object, _mockPostService.Object, _mockUserService.Object, _mockCommentService.Object);

			//Act
			var result = await controller.AddComment(ConstIds.ExampleUserId, ConstIds.ExamplePostId, _commentToAdd);

			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result.Result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockPostService.Verify();
		}
	}
}
