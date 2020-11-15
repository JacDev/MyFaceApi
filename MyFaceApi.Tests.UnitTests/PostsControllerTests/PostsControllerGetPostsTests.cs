﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyFaceApi.Controllers;
using MyFaceApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MyFaceApi.Tests.UnitTests.PostsControllerTests
{
	public class PostsControllerGetPostsTests : PostsControllerPreparation
	{
		public PostsControllerGetPostsTests() : base()
		{
		}
		[Fact]
		public void GetPosts_ReturnsAnActionResult_WithAListOfPosts()
		{
			//Arrange
			var user = GetTestUserData().ElementAt(0);

			_mockUserRepo.Setup(repo => repo.CheckIfUserExists(It.IsAny<Guid>()))
				.Returns(true)
				.Verifiable();
			_mockPostRepo.Setup(repo => repo.GetUserPosts(It.IsAny<Guid>()))
				.Returns(user.Posts.ToList())
				.Verifiable();

			var controller = new PostsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper);

			//Act
			var result = controller.GetPosts(user.Id.ToString());

			//Assert
			var actionResult = Assert.IsType<OkObjectResult>(result.Result);
			var model = Assert.IsType<List<Post>>(actionResult.Value);

			Assert.Equal(user.Posts.Count, model.Count);
			_mockUserRepo.Verify();
			_mockPostRepo.Verify();
		}
		[Fact]
		public void GetPosts_ReturnsBadRequestResult_WhenTheUserIdIsInvalid()
		{
			//Arrange
			var controller = new PostsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper);

			//Act
			var result = controller.GetPosts("InvalidGuid");

			//Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
			Assert.Equal("InvalidGuid is not valid Guid.", badRequestResult.Value);
		}
		[Fact]
		public void GetPosts_ReturnsNotFoundObjectResult_WhenTheUserDoesntExist()
		{
			//Arrange
			_mockUserRepo.Setup(repo => repo.CheckIfUserExists(It.IsAny<Guid>()))
				.Returns(false)
				.Verifiable();

			var controller = new PostsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper);

			//Act
			var result = controller.GetPosts(_exaplePostGuid);

			//Assert
			var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
			Assert.Equal($"User: {_exaplePostGuid} not found.", notFoundResult.Value);
			_mockUserRepo.Verify();
			_mockPostRepo.Verify();
		}
		[Fact]
		public void GetPosts_ReturnsInternalServerError_WhenExceptionThrownInRepository()
		{
			//Arrange
			_mockUserRepo.Setup(repo => repo.CheckIfUserExists(It.IsAny<Guid>()))
				.Returns(true)
				.Verifiable();
			_mockPostRepo.Setup(repo => repo.GetUserPosts(It.IsAny<Guid>()))
				.Throws(new ArgumentNullException(nameof(_exaplePostGuid)))
				.Verifiable();


			var controller = new PostsController(_loggerMock.Object, _mockPostRepo.Object, _mockUserRepo.Object, _mapper);

			//Act
			var result = controller.GetPosts(_exaplePostGuid);
			//Assert
			var internalServerErrorResult = Assert.IsType<StatusCodeResult>(result.Result);
			Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
			_mockUserRepo.Verify();
			_mockPostRepo.Verify();
		}
	}
}