using MyFaceApi.Api.Models.FriendsRelationModels;
using System;
using Xunit;
using AutoFixture;
using Moq;
using MyFaceApi.Api.DataAccess.Entities;
using MyFaceApi.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace MyFaceApi.Tests.UnitTests.FriendsRelationsControllerTests
{
	public class FriendsRelationsControllerAddRelationTests : FriendsRelationsControllerTestsPreparation
	{
		protected readonly FriendsRelationToAdd _relationToAdd;
		public FriendsRelationsControllerAddRelationTests() : base()
		{
			_relationToAdd = _fixture.Create<FriendsRelationToAdd>();
		}
		[Fact]
		public async void AddRelation_ReturnsCreatedAtRouteResult_WithRelationData()
		{
			//Arrange
			_mockUserRepo.Setup(repo => repo.CheckIfUserExists(It.IsAny<Guid>()))
				.ReturnsAsync(true)
				.Verifiable();

			FriendsRelation relationEntity = _mapper.Map<FriendsRelation>(_relationToAdd);
			relationEntity.UserId = new Guid(ConstIds.ExampleUserId);
			relationEntity.FriendId = new Guid(ConstIds.ExampleFromWhoId);

			_mockRelationRepo.Setup(repo => repo.AddRelationAsync(It.IsAny<FriendsRelation>()))
				.ReturnsAsync(relationEntity)
				.Verifiable();

			var controller = new FriendsRelationsController(_loggerMock.Object, _mockRelationRepo.Object, _mapper, _mockUserRepo.Object);

			//Act
			var result = await controller.AddRelation(ConstIds.ExampleUserId, _relationToAdd);

			//Assert
			var redirectToActionResult = Assert.IsType<CreatedAtRouteResult>(result.Result);
			Assert.Equal(ConstIds.ExampleUserId, redirectToActionResult.RouteValues["userId"].ToString());
			Assert.Equal(ConstIds.ExampleFromWhoId, redirectToActionResult.RouteValues["friendId"].ToString());
			Assert.Equal("GetRelation", redirectToActionResult.RouteName);
			Assert.IsType<FriendsRelation>(redirectToActionResult.Value);
			_mockUserRepo.Verify();
			_mockRelationRepo.Verify();
		}
		[Fact]
		public async void AddRelation_ReturnsBadRequestObjectResult_WhenTheUserIdIsInvalid()
		{
			//Arrange
			var controller = new FriendsRelationsController(_loggerMock.Object, _mockRelationRepo.Object, _mapper, _mockUserRepo.Object);

			//Act
			var result = await controller.AddRelation(ConstIds.InvalidGuid, _relationToAdd);

			//Assert
			var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result.Result);
			Assert.Equal($"{ConstIds.InvalidGuid} is not valid Guid.", badRequestObjectResult.Value);
		}
		[Theory]
		[InlineData(true, false)]
		[InlineData(false, true)]
		public async void AddRelation_ReturnsNotFoundObjectResult_WhenTheUserOrFriendDoesntExist(bool doesTheUserExist, bool doesTheFriendExist)
		{
			//Arrange
			_mockUserRepo.Setup(repo => repo.CheckIfUserExists(new Guid(ConstIds.ExampleUserId)))
				.ReturnsAsync(doesTheUserExist)
				.Verifiable();
			if (doesTheUserExist)
			{
			_mockUserRepo.Setup(repo => repo.CheckIfUserExists(It.IsNotIn<Guid>(new Guid(ConstIds.ExampleUserId))))
				.ReturnsAsync(doesTheFriendExist)
				.Verifiable();
			}

			var controller = new FriendsRelationsController(_loggerMock.Object, _mockRelationRepo.Object, _mapper, _mockUserRepo.Object);

			//Act
			var result = await controller.AddRelation(ConstIds.ExampleUserId, _relationToAdd);

			//Assert
			var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result.Result);
			Assert.Equal($"User: {ConstIds.ExampleUserId} or friend: {_relationToAdd.FriendId} not found.", notFoundObjectResult.Value);
			_mockUserRepo.Verify();
		}
		[Fact]
		public async void AddPost_ReturnsInternalServerErrorResult_WhenExceptionThrownInRepository()
		{
			//Arrange
			_mockUserRepo.Setup(repo => repo.CheckIfUserExists(It.IsAny<Guid>()))
				.Throws(new ArgumentNullException(nameof(Guid)))
				.Verifiable();

			var controller = new FriendsRelationsController(_loggerMock.Object, _mockRelationRepo.Object, _mapper, _mockUserRepo.Object);

			//Act
			var result = await controller.AddRelation(ConstIds.ExampleUserId, _relationToAdd);
			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result.Result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockUserRepo.Verify();
		}
	}
}
