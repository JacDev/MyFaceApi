using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyFaceApi.Api.Controllers;
using MyFaceApi.Api.DataAccess.Entities;
using System;
using System.Collections.Generic;
using Xunit;
using AutoFixture;
using MyFaceApi.Api.DataAccess.ModelsBasicInfo;
using MyFaceApi.Api.DboModels;
using System.Linq;

namespace MyFaceApi.Tests.UnitTests.FriendsRelationsControllerTests
{
	public class FriendsRelationsControllerGetUserRelationsTests : FriendsRelationsControllerTestsPreparation
	{
	private class PagedList<T> : List<T>
	{
		public string NextPageLink { get; set; }
		public string PreviousPageLink { get; set; }
		public int CurrentPage { get; set; }
		public int TotalPages { get; set; }
		public int PageSize { get; set; }
		public int TotalCount { get; set; }
		public bool HasPrevious => (CurrentPage > 1);
		public bool HasNext => (CurrentPage < TotalPages);

		public PagedList()
		{

		}
		public PagedList(List<T> items, int count, int pageNumber, int pageSize)
		{
			TotalCount = count;
			PageSize = pageSize;
			CurrentPage = pageNumber;
			TotalPages = (int)Math.Ceiling(count / (double)pageSize);
			if (items != null)
			{
				AddRange(items);
			}
		}
		public static PagedList<T> Create(List<T> source, int pageNumber, int pageSize, int skip)
		{
			if (source != null)
			{
				var count = source.Count;
				var items = source.Skip(skip).Take(pageSize).ToList();
				return new PagedList<T>(items, count, pageNumber, pageSize);
			}
			else
			{
				return new PagedList<T>(null, 0, 0, 0);
			}
		}
	}
	public FriendsRelationsControllerGetUserRelationsTests() : base()
		{
		}
		[Fact]
		public async void GetUserRelations_ReturnsOkObjectResult_WithAListOfRelationsData()
		{
			//Arrange
			List<Guid> userRelations = new List<Guid>();
			_fixture.AddManyTo(userRelations, 5);
			List<BasicUserData> usersToReturn = new List<BasicUserData>();
			_fixture.AddManyTo(usersToReturn, 5);	

			_mockUserRepo.Setup(repo => repo.CheckIfUserExists(It.IsAny<Guid>()))
				.ReturnsAsync(true)
				.Verifiable();
			_mockRelationRepo.Setup(repo => repo.GetUserFriends(It.IsAny<Guid>()))
				.Returns(userRelations)
				.Verifiable();

			var controller = new FriendsRelationsController(_loggerMock.Object, _mockRelationRepo.Object, _mapper, _mockUserRepo.Object);

			//Act
			var result = await controller.GetUserFriends(ConstIds.ExampleUserId, _paginationsParams);

			//Assert
			var actionResult = Assert.IsType<OkObjectResult>(result.Result);
			var model = Assert.IsType<CollectionWithPaginationData<BasicUserData>>(actionResult.Value);

			Assert.Equal(userRelations.Count, model.Collection.Count);
			_mockUserRepo.Verify();
			_mockRelationRepo.Verify();
		}
		[Fact]
		public async void GetUserRelations_ReturnsBadRequestObjectResult_WhenTheUserIdIsInvalid()
		{
			var controller = new FriendsRelationsController(_loggerMock.Object, _mockRelationRepo.Object, _mapper, _mockUserRepo.Object);

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
			_mockUserRepo.Setup(repo => repo.CheckIfUserExists(It.IsAny<Guid>()))
				.ReturnsAsync(false)
				.Verifiable();

			var controller = new FriendsRelationsController(_loggerMock.Object, _mockRelationRepo.Object, _mapper, _mockUserRepo.Object);

			//Act
			var result = await controller.GetUserFriends(ConstIds.ExampleUserId, _paginationsParams);

			//Assert
			var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result.Result);
			Assert.Equal($"User: {ConstIds.ExampleUserId} not found.", notFoundObjectResult.Value);
			_mockUserRepo.Verify();
		}
		[Fact]
		public async void GetUserRelations_ReturnsInternalServerErrorResult_WhenExceptionThrownInRepository()
		{
			//Arrange
			_mockUserRepo.Setup(repo => repo.CheckIfUserExists(It.IsAny<Guid>()))
				.Throws(new ArgumentNullException(nameof(Guid)))
				.Verifiable();

			var controller = new FriendsRelationsController(_loggerMock.Object, _mockRelationRepo.Object, _mapper, _mockUserRepo.Object);

			//Act
			var result = await controller.GetUserFriends(ConstIds.ExampleUserId, _paginationsParams);

			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result.Result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockUserRepo.Verify();
		}
	}
}
