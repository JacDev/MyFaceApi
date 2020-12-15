using System;
using Xunit;
using AutoFixture;
using Moq;
using MyFaceApi.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using MyFaceApi.Api.Application.DtoModels.FriendsRelation;

namespace MyFaceApi.Tests.UnitTests.FriendsRelationsControllerTests
{
	public class FriendsRelationsControllerAddRelationTests : FriendsRelationsControllerTestsPreparation
	{
		protected readonly FriendsRelationToAddDto _relationToAdd;
		public FriendsRelationsControllerAddRelationTests() : base()
		{
			_relationToAdd = _fixture.Create<FriendsRelationToAddDto>();
		}
		[Fact]
		public async void AddRelation_ReturnsCreatedAtRouteResult_WithRelationData()
		{
			//Arrange
			_mockUserService.Setup(Service => Service.CheckIfUserExists(It.IsAny<Guid>()))
				.ReturnsAsync(true)
				.Verifiable();

			FriendsRelationDto relationEntity = _mapper.Map<FriendsRelationDto>(_relationToAdd);
			relationEntity.UserId = new Guid(ConstIds.ExampleUserId);
			relationEntity.FriendId = new Guid(ConstIds.ExampleFromWhoId);

			_mockRelationService.Setup(Service => Service.AddRelationAsync(It.IsAny<Guid>(), It.IsAny<FriendsRelationToAddDto>()))
				.ReturnsAsync(relationEntity)
				.Verifiable();

			var controller = new FriendsRelationsController(_loggerMock.Object, _mockUserService.Object, _mockRelationService.Object);

			//Act
			var result = await controller.AddRelation(ConstIds.ExampleUserId, _relationToAdd);

			//Assert
			var redirectToActionResult = Assert.IsType<CreatedAtRouteResult>(result.Result);
			Assert.Equal(ConstIds.ExampleUserId, redirectToActionResult.RouteValues["userId"].ToString());
			Assert.Equal(ConstIds.ExampleFromWhoId, redirectToActionResult.RouteValues["friendId"].ToString());
			Assert.Equal("GetRelation", redirectToActionResult.RouteName);
			Assert.IsType<FriendsRelationDto>(redirectToActionResult.Value);
			_mockUserService.Verify();
			_mockRelationService.Verify();
		}
		[Fact]
		public async void AddRelation_ReturnsBadRequestObjectResult_WhenTheUserIdIsInvalid()
		{
			//Arrange
			var controller = new FriendsRelationsController(_loggerMock.Object, _mockUserService.Object, _mockRelationService.Object);

			//Act
			var result = await controller.AddRelation(ConstIds.InvalidGuid, _relationToAdd);

			//Assert
			var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result.Result);
			Assert.Equal($"{ConstIds.InvalidGuid} is not valid guid.", badRequestObjectResult.Value);
		}
		[Theory]
		[InlineData(true, false)]
		[InlineData(false, true)]
		public async void AddRelation_ReturnsNotFoundObjectResult_WhenTheUserOrFriendDoesntExist(bool doesTheUserExist, bool doesTheFriendExist)
		{
			//Arrange
			_mockUserService.Setup(Service => Service.CheckIfUserExists(new Guid(ConstIds.ExampleUserId)))
				.ReturnsAsync(doesTheUserExist)
				.Verifiable();
			if (doesTheUserExist)
			{
			_mockUserService.Setup(Service => Service.CheckIfUserExists(It.IsNotIn<Guid>(new Guid(ConstIds.ExampleUserId))))
				.ReturnsAsync(doesTheFriendExist)
				.Verifiable();
			}

			var controller = new FriendsRelationsController(_loggerMock.Object, _mockUserService.Object, _mockRelationService.Object);

			//Act
			var result = await controller.AddRelation(ConstIds.ExampleUserId, _relationToAdd);

			//Assert
			var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result.Result);
			Assert.Equal($"User: {ConstIds.ExampleUserId} or friend: {_relationToAdd.FriendId} not found.", notFoundObjectResult.Value);
			_mockUserService.Verify();
		}
		[Fact]
		public async void AddPost_ReturnsInternalServerErrorResult_WhenExceptionThrownInService()
		{
			//Arrange
			_mockUserService.Setup(Service => Service.CheckIfUserExists(It.IsAny<Guid>()))
				.Throws(new ArgumentNullException(nameof(Guid)))
				.Verifiable();

			var controller = new FriendsRelationsController(_loggerMock.Object, _mockUserService.Object, _mockRelationService.Object);

			//Act
			var result = await controller.AddRelation(ConstIds.ExampleUserId, _relationToAdd);
			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result.Result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockUserService.Verify();
		}
	}
}
