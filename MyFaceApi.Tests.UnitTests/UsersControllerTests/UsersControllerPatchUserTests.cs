using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Moq;
using MyFaceApi.Controllers;
using MyFaceApi.Entities;
using System;
using System.Linq;
using Xunit;

namespace MyFaceApi.Tests.UnitTests.UsersControllerTests
{
	public class UsersControllerPatchUserTests : UsersControllerPreparation
	{
		public UsersControllerPatchUserTests() : base()
		{
		}
		[Fact]
		public async void PartiallyUpdateUser_ReturnsNoContentResult_WhenTheUserHasBeenUpdated()
		{
			//Arrange
			var userInRepo = GetTestUserData().ElementAt(0);
			_mockRepo.Setup(repo => repo.GetUserAsync(userInRepo.Id))
				.ReturnsAsync(userInRepo)
				.Verifiable();
			_mockRepo.Setup(repo => repo.UpdateUserAsync(userInRepo))
				.Verifiable();
			var controller = new UsersController(_loggerMock.Object, _mockRepo.Object, _mapper);

			var objectValidator = new Mock<IObjectModelValidator>();
			objectValidator.Setup(o => o.Validate(It.IsAny<ActionContext>(),
											  It.IsAny<ValidationStateDictionary>(),
											  It.IsAny<string>(),
											  It.IsAny<Object>()));
			controller.ObjectValidator = objectValidator.Object;

			//Act
			var result = await controller.PartiallyUpdateUser(userInRepo.Id.ToString(), GetJsonPatchDocument());

			//Assert
			var noContentResult = Assert.IsType<NoContentResult>(result);
			Assert.Equal("Changed", userInRepo.FirstName);
			_mockRepo.Verify();
		}
		[Fact]
		public async void PartiallyUpdateUser_ReturnsBadRequestResult_WhenTheUserIdIsInvalid()
		{
			//Arrange
			var controller = new UsersController(_loggerMock.Object, _mockRepo.Object, _mapper);

			//Act
			var result = await controller.PartiallyUpdateUser("InvalidGuid", GetJsonPatchDocument());

			//Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal("InvalidGuid is not valid Guid.", badRequestResult.Value);
		}
		[Fact]
		public async void PartiallyUpdateUser_NotFoundRequest_WhenTheUserDataIsNotInTheDatabase()
		{
			Guid testUserGuid = new Guid("24610263-CEE4-4231-97DF-904EE6437278");
			_mockRepo.Setup(repo => repo.GetUserAsync(testUserGuid))
				.ReturnsAsync((User)null)
				.Verifiable();

			var controller = new UsersController(_loggerMock.Object, _mockRepo.Object, _mapper);

			//Act
			var result = await controller.PartiallyUpdateUser(testUserGuid.ToString(), GetJsonPatchDocument());

			//Assert
			var notFoundResult = Assert.IsType<NotFoundResult>(result);
			_mockRepo.Verify();
		}
		[Fact]
		public async void PartiallyUpdateUser_ReturnsInternalServerError_WhenExceptionThrownInRepository()
		{
			//Arrange
			Guid testUserGuid = new Guid("24610263-CEE4-4231-97DF-904EE6437278");
			_mockRepo.Setup(repo => repo.GetUserAsync(testUserGuid))
				.Throws(new ArgumentNullException(nameof(testUserGuid)))
				.Verifiable();

			var controller = new UsersController(_loggerMock.Object, _mockRepo.Object, _mapper);

			//Act
			var result = await controller.PartiallyUpdateUser(testUserGuid.ToString(), GetJsonPatchDocument());

			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockRepo.Verify();
		}
	}
}
