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
	public class FriendsRelationsControllerGetRelationTests : FriendsRelationsControllerTestsPreparation
	{
		public FriendsRelationsControllerGetRelationTests() : base()
		{
		}
		[Fact]
		public void GetRelation_ReturnsOkObjectResult_WithFriendsRelationData()
		{
			//Arrange
			var relation = GetTestRelationData().ElementAt(0);

			_mockRelationRepo.Setup(repo => repo.GetFriendRelation(It.IsAny<Guid>(), It.IsAny<Guid>()))
				.Returns(relation)
				.Verifiable();

			var controller = new FriendsRelationsController(_loggerMock.Object, _mockRelationRepo.Object, _mapper, _mockUserRepo.Object);

			//Act
			var result = controller.GetRelation(ConstIds.ExampleUserId, ConstIds.ExampleFromWhoId);

			//Assert
			var actionResult = Assert.IsType<OkObjectResult>(result.Result);
			var model = Assert.IsType<FriendsRelation>(actionResult.Value);
			Assert.Equal(relation, model);
			_mockRelationRepo.Verify();
		}
		[Theory]
		[InlineData(ConstIds.InvalidGuid, ConstIds.ExampleNotificationId)]
		[InlineData(ConstIds.ExampleUserId, ConstIds.InvalidGuid)]
		public void GetRelation_ReturnsBadRequestObjectResult_WhenTheUserOrFriendIdIsInvalid(string testUserId, string testFriendId)
		{
			//Arrange
			var controller = new FriendsRelationsController(_loggerMock.Object, _mockRelationRepo.Object, _mapper, _mockUserRepo.Object);

			//Act
			var result = controller.GetRelation(testUserId, testFriendId);

			//Assert
			var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result.Result);
			Assert.Equal($"{testUserId} or {testFriendId} is not valid Guid.", badRequestObjectResult.Value);
		}
		[Fact]
		public void GetRelation_ReturnsInternalServerErrorResult_WhenExceptionThrownInRepository()
		{
			//Arrange
			_mockRelationRepo.Setup(repo => repo.GetFriendRelation(It.IsAny<Guid>(), It.IsAny<Guid>()))
				.Throws(new ArgumentNullException(nameof(Guid)))
				.Verifiable();

			var controller = new FriendsRelationsController(_loggerMock.Object, _mockRelationRepo.Object, _mapper, _mockUserRepo.Object);
			//Act
			var result = controller.GetRelation(ConstIds.ExampleUserId, ConstIds.ExampleFromWhoId);
			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result.Result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockRelationRepo.Verify();
		}
	}
}
