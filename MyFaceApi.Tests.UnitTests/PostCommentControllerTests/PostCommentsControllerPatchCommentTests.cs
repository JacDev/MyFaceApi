using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Moq;
using MyFaceApi.Controllers;
using MyFaceApi.Entities;
using System;
using System.Linq;
using Xunit;

namespace MyFaceApi.Tests.UnitTests.PostCommentControllerTests
{
	public class PostCommentsControllerPatchCommentTests: PostCommentsControllerPreparation
	{
		public PostCommentsControllerPatchCommentTests() : base()
		{
		}
		[Fact]
		public async void PartiallyUpdateComment_ReturnsNoContentResult_WhenTheCommentHasBeenUpdated()
		{
			//Arrange
			var comment = GetTestPostData().ElementAt(0);

			_mockCommentRepo.Setup(repo => repo.GetComment(It.IsAny<Guid>()))
				.Returns(comment)
				.Verifiable();
			_mockCommentRepo.Setup(repo => repo.UpdateComment(It.IsAny<PostComment>()))
				.Verifiable();

			var controller = new PostCommentsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper, _mockCommentRepo.Object);

			var objectValidator = new Mock<IObjectModelValidator>();
			objectValidator.Setup(o => o.Validate(It.IsAny<ActionContext>(),
											  It.IsAny<ValidationStateDictionary>(),
											  It.IsAny<string>(),
											  It.IsAny<Object>()));
			controller.ObjectValidator = objectValidator.Object;

			//Act
			var result = await controller.PartiallyUpdateComment(ConstIds.ExampleCommentId, GetJsonPatchDocument());

			//Assert
			var noContentResult = Assert.IsType<NoContentResult>(result);
			Assert.Equal("Changed", comment.Text);
			_mockCommentRepo.Verify();
		}
		[Fact]
		public async void PartiallyUpdateComment_ReturnsBadRequestObjectResult_WhenTheCommentIdIsInvalid()
		{
			//Arrange
			var controller = new PostCommentsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper, _mockCommentRepo.Object);

			//Act
			var result = await controller.PartiallyUpdateComment(ConstIds.InvalidGuid, GetJsonPatchDocument());

			//Assert
			var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal($"{ConstIds.InvalidGuid} is not valid Guid.", badRequestObjectResult.Value);
		}
		[Fact]
		public async void PartiallyUpdateComment_ReturnsNotFoundRequest_WhenTheCommentDataIsNotInTheDatabase()
		{
			//Arrange
			_mockCommentRepo.Setup(repo => repo.GetComment(It.IsAny<Guid>()))
				.Returns((PostComment)null)
				.Verifiable();

			var controller = new PostCommentsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper, _mockCommentRepo.Object);

			//Act
			var result = await controller.PartiallyUpdateComment(ConstIds.ExampleCommentId, GetJsonPatchDocument());

			//Assert
			var notFoundResult = Assert.IsType<NotFoundResult>(result);
			_mockCommentRepo.Verify();
		}
		[Fact]
		public async void PartiallyUpdateComment_ReturnsInternalServerErrorResult_WhenExceptionThrownInRepository()
		{
			//Arrange
			_mockCommentRepo.Setup(repo => repo.GetComment(It.IsAny<Guid>()))
				.Throws(new ArgumentNullException(nameof(Guid)))
				.Verifiable();

			var controller = new PostCommentsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper, _mockCommentRepo.Object);

			//Act
			var result = await controller.PartiallyUpdateComment(ConstIds.ExampleCommentId, GetJsonPatchDocument());

			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockCommentRepo.Verify();
		}
	}
}
