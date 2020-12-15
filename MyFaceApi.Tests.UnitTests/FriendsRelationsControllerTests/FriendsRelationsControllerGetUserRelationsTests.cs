using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyFaceApi.Api.Controllers;
using System;
using System.Collections.Generic;
using Xunit;
using AutoFixture;
using System.Linq;

namespace MyFaceApi.Tests.UnitTests.FriendsRelationsControllerTests
{
	public class FriendsRelationsControllerGetUserRelationsTests : FriendsRelationsControllerTestsPreparation
	{
	public FriendsRelationsControllerGetUserRelationsTests() : base()
		{
		}
		[Fact]
		public async void GetUserRelations_ReturnsOkObjectResult_WithAListOfRelationsData()
		{
			//Arrange
			//List<Guid> userRelations = new List<Guid>();
			//_fixture.AddManyTo(userRelations, 5);
			//List<BasicUserData> usersToReturn = new List<BasicUserData>();
			//_fixture.AddManyTo(usersToReturn, 5);	

			//_mockUserService.Setup(Service => Service.CheckIfUserExists(It.IsAny<Guid>()))
			//	.ReturnsAsync(true)
			//	.Verifiable();
			//_mockRelationService.Setup(Service => Service.GetUserFriends(It.IsAny<Guid>()))
			//	.Returns(userRelations)
			//	.Verifiable();

			//var controller = new FriendsRelationsController(_loggerMock.Object, _mockRelationService.Object, _mapper, _mockUserService.Object);

			////Act
			//var result = await controller.GetUserFriends(ConstIds.ExampleUserId, _paginationsParams);

			////Assert
			//var actionResult = Assert.IsType<OkObjectResult>(result.Result);
			//var model = Assert.IsType<CollectionWithPaginationData<BasicUserData>>(actionResult.Value);

			//Assert.Equal(userRelations.Count, model.Collection.Count);
			//_mockUserService.Verify();
			//_mockRelationService.Verify();
		}
		[Fact]
		public async void GetUserRelations_ReturnsBadRequestObjectResult_WhenTheUserIdIsInvalid()
		{
			var controller = new FriendsRelationsController(_loggerMock.Object, _mockUserService.Object, _mockRelationService.Object);

			//Act
			var result = await controller.GetUserFriends(ConstIds.InvalidGuid, _paginationsParams);

			//Assert
			var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result.Result);
			Assert.Equal($"{ConstIds.InvalidGuid} is not valid guid.", badRequestObjectResult.Value);
		}
		[Fact]
		public async void GetUserRelations_ReturnsNotFoundObjectResult_WhenTheUserDoesntExist()
		{
			//Arrange
			_mockUserService.Setup(Service => Service.CheckIfUserExists(It.IsAny<Guid>()))
				.ReturnsAsync(false)
				.Verifiable();

			var controller = new FriendsRelationsController(_loggerMock.Object, _mockUserService.Object, _mockRelationService.Object);

			//Act
			var result = await controller.GetUserFriends(ConstIds.ExampleUserId, _paginationsParams);

			//Assert
			var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result.Result);
			Assert.Equal($"User: {ConstIds.ExampleUserId} not found.", notFoundObjectResult.Value);
			_mockUserService.Verify();
		}
		[Fact]
		public async void GetUserRelations_ReturnsInternalServerErrorResult_WhenExceptionThrownInService()
		{
			//Arrange
			_mockUserService.Setup(Service => Service.CheckIfUserExists(It.IsAny<Guid>()))
				.Throws(new ArgumentNullException(nameof(Guid)))
				.Verifiable();

			var controller = new FriendsRelationsController(_loggerMock.Object, _mockUserService.Object, _mockRelationService.Object);

			//Act
			var result = await controller.GetUserFriends(ConstIds.ExampleUserId, _paginationsParams);

			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result.Result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockUserService.Verify();
		}
	}
}
