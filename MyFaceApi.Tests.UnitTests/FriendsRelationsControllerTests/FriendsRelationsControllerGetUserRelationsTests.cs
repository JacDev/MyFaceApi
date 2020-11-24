using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyFaceApi.Api.Controllers;
using MyFaceApi.Api.DataAccess.Entities;
using System;
using System.Collections.Generic;
using Xunit;

namespace MyFaceApi.Tests.UnitTests.FriendsRelationsControllerTests
{
	public class FriendsRelationsControllerGetUserRelationsTests : FriendsRelationsControllerTestsPreparation
	{
		public FriendsRelationsControllerGetUserRelationsTests() : base()
		{
		}
		[Fact]
		public void GetUserRelations_ReturnsOkObjectResult_WithAListOfRelationsData()
		{
			//Arrange
			var userRelations = GetTestRelationData();

			_mockUserRepo.Setup(repo => repo.CheckIfUserExists(It.IsAny<Guid>()))
				.Returns(true)
				.Verifiable();
			_mockRelationRepo.Setup(repo => repo.GetUserRelationships(It.IsAny<Guid>()))
				.Returns(userRelations)
				.Verifiable();

			var controller = new FriendsRelationsController(_loggerMock.Object, _mockRelationRepo.Object, _mapper, _mockUserRepo.Object);

			//Act
			var result = controller.GetUserRelations(ConstIds.ExampleUserId);

			//Assert
			var actionResult = Assert.IsType<OkObjectResult>(result.Result);
			var model = Assert.IsType<List<FriendsRelation>>(actionResult.Value);

			Assert.Equal(userRelations.Count, model.Count);
			_mockUserRepo.Verify();
			_mockRelationRepo.Verify();
		}
		[Fact]
		public void GetUserRelations_ReturnsBadRequestObjectResult_WhenTheUserIdIsInvalid()
		{
			var controller = new FriendsRelationsController(_loggerMock.Object, _mockRelationRepo.Object, _mapper, _mockUserRepo.Object);

			//Act
			var result = controller.GetUserRelations(ConstIds.InvalidGuid);

			//Assert
			var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result.Result);
			Assert.Equal($"{ConstIds.InvalidGuid} is not valid guid.", badRequestObjectResult.Value);
		}
		[Fact]
		public void GetUserRelations_ReturnsNotFoundObjectResult_WhenTheUserDoesntExist()
		{
			//Arrange
			_mockUserRepo.Setup(repo => repo.CheckIfUserExists(It.IsAny<Guid>()))
				.Returns(false)
				.Verifiable();

			var controller = new FriendsRelationsController(_loggerMock.Object, _mockRelationRepo.Object, _mapper, _mockUserRepo.Object);

			//Act
			var result = controller.GetUserRelations(ConstIds.ExampleUserId);

			//Assert
			var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result.Result);
			Assert.Equal($"User: {ConstIds.ExampleUserId} not found.", notFoundObjectResult.Value);
			_mockUserRepo.Verify();
		}
		[Fact]
		public void GetUserRelations_ReturnsInternalServerErrorResult_WhenExceptionThrownInRepository()
		{
			//Arrange
			_mockUserRepo.Setup(repo => repo.CheckIfUserExists(It.IsAny<Guid>()))
				.Throws(new ArgumentNullException(nameof(Guid)))
				.Verifiable();

			var controller = new FriendsRelationsController(_loggerMock.Object, _mockRelationRepo.Object, _mapper, _mockUserRepo.Object);

			//Act
			var result = controller.GetUserRelations(ConstIds.ExampleUserId);

			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result.Result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockUserRepo.Verify();
		}
	}
}
