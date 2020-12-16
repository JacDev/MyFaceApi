using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyFaceApi.Api.Controllers;
using System;
using Xunit;
using AutoFixture;
using MyFaceApi.Api.Application.DtoModels.PostReaction;

namespace MyFaceApi.Api.Tests.UnitTests.PostReactionsControllerTests
{
	public class PostReactionsControllerAddReactionTests : PostReactionsPreparation
	{
		protected readonly PostReactionToAddDto _postReactionToAdd;
		public PostReactionsControllerAddReactionTests() : base()
		{
			_postReactionToAdd = _fixture.Create<PostReactionToAddDto>();
		}
		[Fact]
		public async void AddPostReaction_ReturnsCreatedAtRouteResult_WithReactionData()
		{
			//Arrange
			_mockPostService.Setup(Service => Service.CheckIfPostExists(It.IsAny<Guid>()))
				.Returns(true)
				.Verifiable();
			_mockUserService.Setup(Service => Service.CheckIfUserExists(It.IsAny<Guid>()))
				.ReturnsAsync(true)
				.Verifiable();

			PostReactionDto reactionEntity = _mapper.Map<PostReactionDto>(_postReactionToAdd);
			reactionEntity.Id = new Guid(ConstIds.ExampleReactionId);
			reactionEntity.PostId = new Guid(ConstIds.ExamplePostId);

			_mockReactionService.Setup(Service => Service.AddPostReactionAsync(It.IsAny<Guid>(), It.IsAny<PostReactionToAddDto>()))
				.ReturnsAsync(reactionEntity)
				.Verifiable();

			var controller = new PostReactionsController(_loggerMock.Object, _mockPostService.Object, _mockUserService.Object, _mockReactionService.Object);

			//Act
			var result = await controller.AddPostReaction(ConstIds.ExampleUserId, ConstIds.ExamplePostId, _postReactionToAdd);

			//Assert
			var redirectToActionResult = Assert.IsType<CreatedAtRouteResult>(result.Result);
			Assert.Equal(ConstIds.ExampleUserId, redirectToActionResult.RouteValues["userId"].ToString());
			Assert.Equal(ConstIds.ExamplePostId, redirectToActionResult.RouteValues["postId"].ToString());
			Assert.Equal(ConstIds.ExampleReactionId, redirectToActionResult.RouteValues["reactionId"].ToString());
			Assert.Equal("GetReaction", redirectToActionResult.RouteName);
			_mockUserService.Verify();
			_mockReactionService.Verify();
			_mockPostService.Verify();
		}
		
		[Theory]
		[InlineData(ConstIds.InvalidGuid, ConstIds.ExamplePostId)]
		[InlineData(ConstIds.ExampleUserId, ConstIds.InvalidGuid)]
		public async void AddPostReaction_ReturnsBadRequestObjectResult_WhenTheUserOrPostIdIsInvalid(string userTestId, string postTestId)
		{
			//Arrange
			var controller = new PostReactionsController(_loggerMock.Object, _mockPostService.Object, _mockUserService.Object, _mockReactionService.Object);
			//Act
			var result = await controller.AddPostReaction(userTestId, postTestId, _postReactionToAdd);

			//Assert
			var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result.Result);
			Assert.Equal($"{userTestId} or {postTestId} is not valid guid.", badRequestObjectResult.Value);
		}
		[Theory]
		[InlineData(true, false)]
		[InlineData(false, true)]
		public async void AddPostReaction_ReturnsNotFoundObjectResult_WhenTheUserOrPostDoesntExist(bool doesTheUserExists, bool doesThePostExists)
		{
			//Arrange
			_mockPostService.Setup(Service => Service.CheckIfPostExists(It.IsAny<Guid>()))
				.Returns(doesThePostExists)
				.Verifiable();

			_mockUserService.Setup(Service => Service.CheckIfUserExists(It.IsAny<Guid>()))
				.ReturnsAsync(doesTheUserExists)
				.Verifiable();

			var controller = new PostReactionsController(_loggerMock.Object, _mockPostService.Object, _mockUserService.Object, _mockReactionService.Object);
			//Act
			var result = await controller.AddPostReaction(ConstIds.ExampleUserId, ConstIds.ExamplePostId, _postReactionToAdd);

			//Assert
			var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
			Assert.Equal($"User: {ConstIds.ExampleUserId} or post {ConstIds.ExamplePostId} not found.", notFoundResult.Value);

			//if post doesnt exist the user will not be checked
			if (doesThePostExists)
			{
				_mockUserService.Verify();
			}
			_mockPostService.Verify();
		}
		[Fact]
		public async void AddPostReaction_ReturnsInternalServerErrorResult_WhenExceptionThrownInService()
		{
			//Arrange
			_mockPostService.Setup(Service => Service.CheckIfPostExists(It.IsAny<Guid>()))
				.Throws(new ArgumentNullException(nameof(Guid)))
				.Verifiable();

			var controller = new PostReactionsController(_loggerMock.Object, _mockPostService.Object, _mockUserService.Object, _mockReactionService.Object);
			//Act
			var result = await controller.AddPostReaction(ConstIds.ExampleUserId, ConstIds.ExamplePostId, _postReactionToAdd);
			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result.Result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockUserService.Verify();
			_mockPostService.Verify();
		}
	}
}
