using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyFaceApi.Api.Controllers;
using System;
using System.Linq;
using Xunit;

namespace MyFaceApi.Tests.UnitTests.FriendsRelationsControllerTests
{
	public class FriendsRelationsControllerDeleteRelationTests : FriendsRelationsControllerTestsPreparation
	{
		public FriendsRelationsControllerDeleteRelationTests() :base()
		{
		}
		[Fact]
		public async void DeleteRelation_ReturnsNoContentResult_WhenTheRelationHasBeenRemoved()
		{
			//Arrange
			var relationToRemove = GetTestRelationData().ElementAt(0);
			_mockUserService.Setup(Service => Service.CheckIfUserExists(It.IsAny<Guid>()))
				.ReturnsAsync(true)
				.Verifiable();
			_mockRelationService.Setup(Service => Service.DeleteRelationAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
				.Verifiable();

			var controller = new FriendsRelationsController(_loggerMock.Object, _mockUserService.Object, _mockRelationService.Object);

			//Act
			var result = await controller.DeleteRelation(ConstIds.ExampleUserId, ConstIds.ExampleFromWhoId);

			//Act
			var noContentResult = Assert.IsType<NoContentResult>(result);
			_mockUserService.Verify();
			_mockRelationService.Verify();
		}
		[Theory]
		[InlineData(ConstIds.InvalidGuid, ConstIds.ExampleNotificationId)]
		[InlineData(ConstIds.ExampleUserId, ConstIds.InvalidGuid)]
		public async void DeleteRelation_ReturnsBadRequestObjectResult_WhenTheUserOrFriendGuidIdIsInvalid(string testUserId, string testFriendId)
		{
			//Arrange
			var controller = new FriendsRelationsController(_loggerMock.Object, _mockUserService.Object, _mockRelationService.Object);

			//Act
			var result = await controller.DeleteRelation(testUserId, testFriendId);

			//Assert
			var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal($"{testUserId} or {testFriendId} is not valid guid.", badRequestObjectResult.Value);
		}
		[Theory]
		[InlineData(true, false)]
		[InlineData(false, true)]
		public async void DeleteRelation_ReturnsNotFoundObjectResult_WhenTheUserOrFriendDoesntExist(bool doesTheUserExist, bool doesTheFriendExist)
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
			var result = await controller.DeleteRelation(ConstIds.ExampleUserId, ConstIds.ExampleFromWhoId);

			//Assert
			var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result);
			Assert.Equal($"User: {ConstIds.ExampleUserId} or friend: {ConstIds.ExampleFromWhoId} not found.", notFoundObjectResult.Value);
			_mockUserService.Verify();
		}
		[Fact]
		public async void DeleteRelation_ReturnsInternalServerErrorResult_WhenExceptionThrownInService()
		{
			//Arrange
			_mockUserService.Setup(Service => Service.CheckIfUserExists(new Guid(ConstIds.ExampleUserId)))
				.Throws(new ArgumentNullException(nameof(Guid)))
				.Verifiable();

			var controller = new FriendsRelationsController(_loggerMock.Object, _mockUserService.Object, _mockRelationService.Object);

			//Act
			var result = await controller.DeleteRelation(ConstIds.ExampleUserId, ConstIds.ExampleFromWhoId);

			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockUserService.Verify();
		}
	}
}
