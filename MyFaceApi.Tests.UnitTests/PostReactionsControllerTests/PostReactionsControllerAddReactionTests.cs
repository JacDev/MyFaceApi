using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyFaceApi.Api.Controllers;
using MyFaceApi.Api.DataAccess.Entities;
using MyFaceApi.Api.Models.PostReactionModels;
using System;
using Xunit;
using AutoFixture;

namespace MyFaceApi.Tests.UnitTests.PostReactionsControllerTests
{
	public class PostReactionsControllerAddReactionTests : PostReactionsPreparation
	{
		protected readonly PostReactionToAdd _postReactionToAdd;
		public PostReactionsControllerAddReactionTests() : base()
		{
			_postReactionToAdd = _fixture.Create<PostReactionToAdd>();
		}
		[Fact]
		public async void AddPostReaction_ReturnsCreatedAtRouteResult_WithReactionData()
		{
			//Arrange
			_mockPostRepo.Setup(repo => repo.CheckIfPostExists(It.IsAny<Guid>()))
				.Returns(true)
				.Verifiable();
			_mockUserRepo.Setup(repo => repo.CheckIfUserExists(It.IsAny<Guid>()))
				.Returns(true)
				.Verifiable();

			PostReaction reactionEntity = _mapper.Map<PostReaction>(_postReactionToAdd);
			reactionEntity.Id = new Guid(ConstIds.ExampleReactionId);
			reactionEntity.PostId = new Guid(ConstIds.ExamplePostId);

			_mockReactionRepo.Setup(repo => repo.AddPostReactionAsync(It.IsAny<PostReaction>()))
				.ReturnsAsync(reactionEntity)
				.Verifiable();

			var controller = new PostReactionsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper, _mockReactionRepo.Object);

			//Act
			var result = await controller.AddPostReaction(ConstIds.ExampleUserId, ConstIds.ExamplePostId, _postReactionToAdd);

			//Assert
			var redirectToActionResult = Assert.IsType<CreatedAtRouteResult>(result.Result);
			Assert.Equal(ConstIds.ExampleUserId, redirectToActionResult.RouteValues["userId"].ToString());
			Assert.Equal(ConstIds.ExamplePostId, redirectToActionResult.RouteValues["postId"].ToString());
			Assert.Equal(ConstIds.ExampleReactionId, redirectToActionResult.RouteValues["commentId"].ToString());
			Assert.Equal("GetReaction", redirectToActionResult.RouteName);
			_mockUserRepo.Verify();
			_mockReactionRepo.Verify();
			_mockPostRepo.Verify();
		}
		
		[Theory]
		[InlineData(ConstIds.InvalidGuid, ConstIds.ExamplePostId)]
		[InlineData(ConstIds.ExampleUserId, ConstIds.InvalidGuid)]
		public async void AddPostReaction_ReturnsBadRequestObjectResult_WhenTheUserOrPostIdIsInvalid(string userTestId, string postTestId)
		{
			//Arrange
			var controller = new PostReactionsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper, _mockReactionRepo.Object);

			//Act
			var result = await controller.AddPostReaction(userTestId, postTestId, _postReactionToAdd);

			//Assert
			var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result.Result);
			Assert.Equal($"{userTestId} or {postTestId} is not valid Guid.", badRequestObjectResult.Value);
		}
		[Theory]
		[InlineData(true, false)]
		[InlineData(false, true)]
		public async void AddPostReaction_ReturnsNotFoundObjectResult_WhenTheUserOrPostDoesntExist(bool doesTheUserExists, bool doesThePostExists)
		{
			//Arrange
			_mockPostRepo.Setup(repo => repo.CheckIfPostExists(It.IsAny<Guid>()))
				.Returns(doesThePostExists)
				.Verifiable();

			_mockUserRepo.Setup(repo => repo.CheckIfUserExists(It.IsAny<Guid>()))
				.Returns(doesTheUserExists)
				.Verifiable();

			var controller = new PostReactionsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper, _mockReactionRepo.Object);

			//Act
			var result = await controller.AddPostReaction(ConstIds.ExampleUserId, ConstIds.ExamplePostId, _postReactionToAdd);

			//Assert
			var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
			Assert.Equal($"User: {ConstIds.ExampleUserId} or post {ConstIds.ExamplePostId} not found.", notFoundResult.Value);

			//if post doesnt exist the user will not be checked
			if (doesThePostExists)
			{
				_mockUserRepo.Verify();
			}
			_mockPostRepo.Verify();
		}
		[Fact]
		public async void AddPostReaction_ReturnsInternalServerErrorResult_WhenExceptionThrownInRepository()
		{
			//Arrange
			_mockPostRepo.Setup(repo => repo.CheckIfPostExists(It.IsAny<Guid>()))
				.Throws(new ArgumentNullException(nameof(Guid)))
				.Verifiable();

			var controller = new PostReactionsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper, _mockReactionRepo.Object);

			//Act
			var result = await controller.AddPostReaction(ConstIds.ExampleUserId, ConstIds.ExamplePostId, _postReactionToAdd);
			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result.Result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockUserRepo.Verify();
			_mockPostRepo.Verify();
		}
	}
}
