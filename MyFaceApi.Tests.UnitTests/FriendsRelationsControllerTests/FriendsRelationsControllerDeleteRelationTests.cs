using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyFaceApi.Controllers;
using MyFaceApi.DataAccess.Entities;
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
			_mockUserRepo.Setup(repo => repo.CheckIfUserExists(It.IsAny<Guid>()))
				.Returns(true)
				.Verifiable();
			_mockRelationRepo.Setup(repo => repo.GetFriendRelation(It.IsAny<Guid>(), It.IsAny<Guid>()))
				.Returns(relationToRemove)
				.Verifiable();
			_mockRelationRepo.Setup(repo => repo.DeleteRelationAsync(It.IsAny<FriendsRelation>()))
				.Verifiable();

			var controller = new FriendsRelationsController(_loggerMock.Object, _mockRelationRepo.Object, _mapper, _mockUserRepo.Object);

			//Act
			var result = await controller.DeleteRelation(ConstIds.ExampleUserId, ConstIds.ExampleFromWhoId);

			//Act
			var noContentResult = Assert.IsType<NoContentResult>(result);
			_mockUserRepo.Verify();
			_mockRelationRepo.Verify();
		}
		[Theory]
		[InlineData(ConstIds.InvalidGuid, ConstIds.ExampleNotificationId)]
		[InlineData(ConstIds.ExampleUserId, ConstIds.InvalidGuid)]
		public async void DeleteRelation_ReturnsBadRequestObjectResult_WhenTheUserOrFriendGuidIdIsInvalid(string testUserId, string testFriendId)
		{
			//Arrange
			var controller = new FriendsRelationsController(_loggerMock.Object, _mockRelationRepo.Object, _mapper, _mockUserRepo.Object);

			//Act
			var result = await controller.DeleteRelation(testUserId, testFriendId);

			//Assert
			var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal($"{testUserId} or {testFriendId} is not valid Guid.", badRequestObjectResult.Value);
		}
		[Theory]
		[InlineData(true, false)]
		[InlineData(false, true)]
		public async void DeleteRelation_ReturnsNotFoundObjectResult_WhenTheUserOrFriendDoesntExist(bool doesTheUserExist, bool doesTheFriendExist)
		{
			//Arrange
			_mockUserRepo.Setup(repo => repo.CheckIfUserExists(new Guid(ConstIds.ExampleUserId)))
				.Returns(doesTheUserExist)
				.Verifiable();
			if (doesTheUserExist)
			{
				_mockUserRepo.Setup(repo => repo.CheckIfUserExists(It.IsNotIn<Guid>(new Guid(ConstIds.ExampleUserId))))
					.Returns(doesTheFriendExist)
					.Verifiable();
			}

			var controller = new FriendsRelationsController(_loggerMock.Object, _mockRelationRepo.Object, _mapper, _mockUserRepo.Object);

			//Act
			var result = await controller.DeleteRelation(ConstIds.ExampleUserId, ConstIds.ExampleFromWhoId);

			//Assert
			var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result);
			Assert.Equal($"User: {ConstIds.ExampleUserId} or friend: {ConstIds.ExampleFromWhoId} not found.", notFoundObjectResult.Value);
			_mockUserRepo.Verify();
		}
		[Fact]
		public async void DeleteRelation_ReturnsInternalServerErrorResult_WhenExceptionThrownInRepository()
		{
			//Arrange
			_mockUserRepo.Setup(repo => repo.CheckIfUserExists(new Guid(ConstIds.ExampleUserId)))
				.Throws(new ArgumentNullException(nameof(Guid)))
				.Verifiable();

			var controller = new FriendsRelationsController(_loggerMock.Object, _mockRelationRepo.Object, _mapper, _mockUserRepo.Object);

			//Act
			var result = await controller.DeleteRelation(ConstIds.ExampleUserId, ConstIds.ExampleFromWhoId);

			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockUserRepo.Verify();
		}
	}
}
