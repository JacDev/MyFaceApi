using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyFaceApi.Api.Controllers;
using MyFaceApi.Api.DataAccess.Entities;
using MyFaceApi.Api.DataAccess.ModelsBasicInfo;
using System;
using Xunit;
using AutoFixture;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MyFaceApi.Tests.UnitTests.UsersControllerTests
{
	public class UsersControllerAddUserTests : UsersControllerPreparation
	{
		protected readonly BasicUserData _userToAdd;
		public UsersControllerAddUserTests() : base()
		{
			_userToAdd = _fixture.Create<BasicUserData>();
		}
		[Fact]
		public async void AddUser_ReturnsCreatedAtRouteResult_WithUserData()
		{
			//Arrange
			_mockUserRepo.Setup(repo => repo.AddUserAcync(It.IsAny<User>()))
				.ReturnsAsync(_mapper.Map<User>(_userToAdd))
				.Verifiable();

			var controller = new UsersController(_loggerMock.Object, _mockUserRepo.Object, _mapper);

			var objectValidator = new Mock<IObjectModelValidator>();
			objectValidator.Setup(o => o.Validate(It.IsAny<ActionContext>(),
											  It.IsAny<ValidationStateDictionary>(),
											  It.IsAny<string>(),
											  It.IsAny<Object>()));
			controller.ObjectValidator = objectValidator.Object;
			//Act
			var result = await controller.AddUser(_userToAdd);

			//Assert
			var redirectToActionResult = Assert.IsType<CreatedAtRouteResult>(result.Result);
			Assert.Equal(_userToAdd.Id, redirectToActionResult.RouteValues["userId"]);
			Assert.Equal("GetUser", redirectToActionResult.RouteName);
			_mockUserRepo.Verify();
		}
		[Fact]
		public async void AddUser_ReturnsInternalServerErrorResult_WhenExceptionThrownInRepository()
		{
			//Arrange
			_mockUserRepo.Setup(repo => repo.AddUserAcync(It.IsAny<User>()))
				.Throws(new ArgumentNullException(nameof(User)))
				.Verifiable();

			var controller = new UsersController(_loggerMock.Object, _mockUserRepo.Object, _mapper);

			var objectValidator = new Mock<IObjectModelValidator>();
			objectValidator.Setup(o => o.Validate(It.IsAny<ActionContext>(),
											  It.IsAny<ValidationStateDictionary>(),
											  It.IsAny<string>(),
											  It.IsAny<Object>()));
			controller.ObjectValidator = objectValidator.Object;

			//Act
			var result = await controller.AddUser(_userToAdd);

			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result.Result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockUserRepo.Verify();
		}
	}
}
