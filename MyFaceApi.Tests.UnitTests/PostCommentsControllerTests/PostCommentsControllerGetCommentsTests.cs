using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyFaceApi.Api.Controllers;
using MyFaceApi.Api.DataAccess.Entities;
using System;
using System.Collections.Generic;
using Xunit;

namespace MyFaceApi.Tests.UnitTests.PostCommentsControllerTests
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
			_mockPostRepo.Setup(repo => repo.CheckIfPostExists(It.IsAny<Guid>()))
				.Returns(true)
				.Verifiable();
			_mockCommentRepo.Setup(repo => repo.GetComments(It.IsAny<Guid>()))
				.Returns(comments)
				.Verifiable();

			var controller = new PostCommentsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper, _mockCommentRepo.Object);

			//Act
			var result = controller.GetComments(ConstIds.ExamplePostId);

			//Assert
			var actionResult = Assert.IsType<OkObjectResult>(result.Result);
			var model = Assert.IsType<List<PostComment>>(actionResult.Value);
			_mockCommentRepo.Verify();
			_mockPostRepo.Verify();
		}
		[Fact]
		public void GetComments_ReturnsBadRequestObjectResult_WhenThePostIdIsInvalid()
		{
			//Arrange
			var controller = new PostCommentsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper, _mockCommentRepo.Object);

			//Act
			var result = controller.GetComments(ConstIds.InvalidGuid);

			//Assert
			var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result.Result);
			Assert.Equal($"{ConstIds.InvalidGuid} is not valid Guid.", badRequestObjectResult.Value);
		}
		[Fact]
		public void GetComments_ReturnsNotFoundObjectResult_WhenThePostDoesntExist()
		{
			//Arrange
			_mockPostRepo.Setup(repo => repo.CheckIfPostExists(It.IsAny<Guid>()))
				.Returns(false)
				.Verifiable();

			var controller = new PostCommentsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper, _mockCommentRepo.Object);

			//Act
			var result = controller.GetComments(ConstIds.ExamplePostId);

			//Assert
			var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result.Result);
			Assert.Equal($"Post: {ConstIds.ExamplePostId} not found.", notFoundObjectResult.Value);
			_mockPostRepo.Verify();
		}
		[Fact]
		public void GetComments_ReturnsInternalServerErrorResult_WhenExceptionThrownInRepository()
		{
			//Arrange
			_mockPostRepo.Setup(repo => repo.CheckIfPostExists(It.IsAny<Guid>()))
				.Throws(new ArgumentNullException(nameof(Guid)))
				.Verifiable();

			var controller = new PostCommentsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper, _mockCommentRepo.Object);

			//Act
			var result = controller.GetComments(ConstIds.ExamplePostId);

			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result.Result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockPostRepo.Verify();
		}
	}
}
