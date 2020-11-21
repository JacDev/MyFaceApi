using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Moq;
using MyFaceApi.Controllers;
using MyFaceApi.DataAccess.Entities;
using MyFaceApi.DataAccess.Enums;
using System;
using System.Linq;
using Xunit;

namespace MyFaceApi.Tests.UnitTests.PostReactionControllerTests
{
	public class PostReactionControllerPatchReactionTests : PostReactionPreparation
	{
		public PostReactionControllerPatchReactionTests() : base()
		{
		}
		[Fact]
		public async void PartiallyUpdatePostReaction_ReturnsNoContentResult_WhenTheReactionHasBeenUpdated()
		{
			//Arrange
			var reaction = GetTestPostData().PostReactions.ElementAt(0);

			_mockReactionRepo.Setup(repo => repo.GetPostReaction(It.IsAny<Guid>()))
				.Returns(reaction)
				.Verifiable();
			_mockReactionRepo.Setup(repo => repo.UpdatePostReactionAsync(It.IsAny<PostReaction>()))
				.Verifiable();

			var controller = new PostReactionController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper, _mockReactionRepo.Object);

			var objectValidator = new Mock<IObjectModelValidator>();
			objectValidator.Setup(o => o.Validate(It.IsAny<ActionContext>(),
											  It.IsAny<ValidationStateDictionary>(),
											  It.IsAny<string>(),
											  It.IsAny<Object>()));
			controller.ObjectValidator = objectValidator.Object;

			//Act
			var result = await controller.PartiallyUpdatePostReaction(ConstIds.ExampleReactionId, GetJsonPatchDocument());

			//Assert
			var noContentResult = Assert.IsType<NoContentResult>(result);
			Assert.Equal(ReactionType.Haha, reaction.Reaction);
			_mockReactionRepo.Verify();
		}
		[Fact]
		public async void PartiallyUpdatePostReaction_ReturnsBadRequestObjectResult_WhenTheReactionIdIsInvalid()
		{
			//Arrange
			var controller = new PostReactionController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper, _mockReactionRepo.Object);

			//Act
			var result = await controller.PartiallyUpdatePostReaction(ConstIds.InvalidGuid, GetJsonPatchDocument());

			//Assert
			var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal($"{ConstIds.InvalidGuid} is not valid Guid.", badRequestObjectResult.Value);
		}
		[Fact]
		public async void PartiallyUpdatePostReaction_NotFoundRequest_WhenTheReactionIsNotInTheDatabase()
		{
			//Arrange
			_mockReactionRepo.Setup(repo => repo.GetPostReaction(It.IsAny<Guid>()))
				.Returns((PostReaction)null)
				.Verifiable();

			var controller = new PostReactionController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper, _mockReactionRepo.Object);

			//Act
			var result = await controller.PartiallyUpdatePostReaction(ConstIds.ExampleReactionId, GetJsonPatchDocument());

			//Assert
			var notFoundResult = Assert.IsType<NotFoundResult>(result);
			_mockReactionRepo.Verify();
		}
		[Fact]
		public async void PartiallyUpdatePostReaction_ReturnsInternalServerErrorResult_WhenExceptionThrownInRepository()
		{
			//Arrange
			_mockReactionRepo.Setup(repo => repo.GetPostReaction(It.IsAny<Guid>()))
				.Throws(new ArgumentNullException(nameof(Guid)))
				.Verifiable();

			var controller = new PostReactionController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper, _mockReactionRepo.Object);

			//Act
			var result = await controller.PartiallyUpdatePostReaction(ConstIds.ExampleReactionId, GetJsonPatchDocument());

			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockReactionRepo.Verify();
		}
	}
}
