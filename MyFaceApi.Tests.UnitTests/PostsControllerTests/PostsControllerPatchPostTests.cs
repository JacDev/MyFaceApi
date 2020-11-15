using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Moq;
using MyFaceApi.Controllers;
using MyFaceApi.Entities;
using System;
using System.Linq;
using Xunit;

namespace MyFaceApi.Tests.UnitTests.PostsControllerTests
{
	public class PostsControllerPatchPostTests : PostsControllerPreparation
	{
		public PostsControllerPatchPostTests() : base()
		{
		}
		[Fact]
		public async void PartiallyUpdatePost_ReturnsNoContentResult_WhenTheUserHasBeenUpdated()
		{
			//Arrange
			var post = GetTestUserData().ElementAt(0).Posts.ElementAt(0);

			_mockPostRepo.Setup(repo => repo.GetPost(It.IsAny<Guid>()))
				.Returns(post)
				.Verifiable();
			_mockPostRepo.Setup(repo => repo.UpdatePostAsync(It.IsAny<Post>()))
				.Verifiable();

			var controller = new PostsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper);

			var objectValidator = new Mock<IObjectModelValidator>();
			objectValidator.Setup(o => o.Validate(It.IsAny<ActionContext>(),
											  It.IsAny<ValidationStateDictionary>(),
											  It.IsAny<string>(),
											  It.IsAny<Object>()));
			controller.ObjectValidator = objectValidator.Object;

			//Act
			var result = await controller.PartiallyUpdatePost(post.Id.ToString(), GetJsonPatchDocument());

			//Assert
			var noContentResult = Assert.IsType<NoContentResult>(result);
			Assert.Equal("Changed", post.Text);
			_mockPostRepo.Verify();
		}
		[Fact]
		public async void PartiallyUpdatePost_ReturnsBadRequestResult_WhenTheUserIdIsInvalid()
		{
			//Arrange
			var controller = new PostsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper);

			//Act
			var result = await controller.PartiallyUpdatePost("InvalidGuid", GetJsonPatchDocument());

			//Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal("InvalidGuid is not valid Guid.", badRequestResult.Value);
		}
		[Fact]
		public async void PartiallyUpdatePost_NotFoundRequest_WhenTheUserDataIsNotInTheDatabase()
		{
			//Arrange
			_mockPostRepo.Setup(repo => repo.GetPost(It.IsAny<Guid>()))
				.Returns((Post)null)
				.Verifiable();

			var controller = new PostsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper);

			//Act
			var result = await controller.PartiallyUpdatePost(_exaplePostGuid, GetJsonPatchDocument());

			//Assert
			var notFoundResult = Assert.IsType<NotFoundResult>(result);
			_mockPostRepo.Verify();
		}
		[Fact]
		public async void PartiallyUpdatePost_ReturnsInternalServerError_WhenExceptionThrownInRepository()
		{
			//Arrange
			_mockPostRepo.Setup(repo => repo.GetPost(It.IsAny<Guid>()))
				.Throws(new ArgumentNullException(nameof(_exaplePostGuid)))
				.Verifiable();

			var controller = new PostsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper);

			//Act
			var result = await controller.PartiallyUpdatePost(_exaplePostGuid, GetJsonPatchDocument());

			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockPostRepo.Verify();
		}
	}
}
