using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Moq;
using MyFaceApi.Api.Controllers;
using MyFaceApi.Api.DataAccess.Entities;
using MyFaceApi.Api.DataAccess.ModelsBasicInfo;
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
			var userInRepo = GetTestUserData();
			_mockUserRepo.Setup(repo => repo.GetUserAsync(It.IsAny<Guid>()))
				.ReturnsAsync(userInRepo)
				.Verifiable();
			_mockUserRepo.Setup(repo => repo.UpdateUserAsync(It.IsAny<User>()))
				.Verifiable();
			var controller = new UsersController(_loggerMock.Object, _mockUserRepo.Object, _mapper);

			var objectValidator = new Mock<IObjectModelValidator>();
			objectValidator.Setup(o => o.Validate(It.IsAny<ActionContext>(),
											  It.IsAny<ValidationStateDictionary>(),
											  It.IsAny<string>(),
											  It.IsAny<Object>()));
			controller.ObjectValidator = objectValidator.Object;

			//Act
			var result = await controller.PartiallyUpdateUser(ConstIds.ExampleUserId, GetJsonPatchDocument());

			//Assert
			var noContentResult = Assert.IsType<NoContentResult>(result);
			Assert.Equal("Changed", userInRepo.FirstName);
			_mockUserRepo.Verify();
		}
		[Fact]
		public async void PartiallyUpdateUser_ReturnsBadRequestObjectResult_WhenTheUserIdIsInvalid()
		{
			//Arrange
			var controller = new UsersController(_loggerMock.Object, _mockUserRepo.Object, _mapper);

			//Act
			var result = await controller.PartiallyUpdateUser(ConstIds.InvalidGuid, GetJsonPatchDocument());

			//Assert
			var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal($"{ConstIds.InvalidGuid} is not valid Guid.", badRequestObjectResult.Value);
		}
		[Fact]
		public async void PartiallyUpdateUser_NotFoundRequest_WhenTheUserDataIsNotInTheDatabase()
		{
			_mockUserRepo.Setup(repo => repo.GetUserAsync(It.IsAny<Guid>()))
				.ReturnsAsync((BasicUserData)null)
				.Verifiable();

			var controller = new UsersController(_loggerMock.Object, _mockUserRepo.Object, _mapper);

			//Act
			var result = await controller.PartiallyUpdateUser(ConstIds.ExampleUserId, GetJsonPatchDocument());

			//Assert
			var notFoundResult = Assert.IsType<NotFoundResult>(result);
			_mockUserRepo.Verify();
		}
		[Fact]
		public async void PartiallyUpdateUser_ReturnsInternalServerErrorResult_WhenExceptionThrownInRepository()
		{
			//Arrange
			_mockUserRepo.Setup(repo => repo.GetUserAsync(It.IsAny<Guid>()))
				.Throws(new ArgumentNullException(nameof(Guid)))
				.Verifiable();

			var controller = new UsersController(_loggerMock.Object, _mockUserRepo.Object, _mapper);

			//Act
			var result = await controller.PartiallyUpdateUser(ConstIds.ExampleUserId, GetJsonPatchDocument());

			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockUserRepo.Verify();
		}
	}
}
