using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using Moq;
using MyFaceApi.AutoMapperProfiles;
using MyFaceApi.Controllers;
using MyFaceApi.Entities;
using MyFaceApi.Models.CommentModels;
using MyFaceApi.Repository.Interfaceses;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;

namespace MyFaceApi.Tests.UnitTests.PostCommentControllerTests
{
	public class PostCommentsControllerPreparation
	{
		protected readonly Mock<IPostCommentRepository> _mockCommentRepo;
		protected readonly Mock<IUserRepository> _mockUserRepo;
		protected readonly Mock<IPostRepository> _mockPostRepo;
		protected readonly Mock<ILogger<PostCommentsController>> _loggerMock;
		protected readonly IMapper _mapper;
		protected readonly string _exampleUserId;
		protected readonly string _examplePostId;
		protected readonly string _exampleCommentId;
		protected readonly string _invalidGuid;
		protected readonly CommentToAdd _commentToAdd;
		protected PostCommentsControllerPreparation()
		{
			//mocking repos
			_mockCommentRepo = new Mock<IPostCommentRepository>();
			_mockUserRepo = new Mock<IUserRepository>();
			_mockPostRepo = new Mock<IPostRepository>();
			//mocking logger
			_loggerMock = new Mock<ILogger<PostCommentsController>>();
			//mocking automapper
			var myProfile = new CommentProfile();
			var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
			_mapper = new Mapper(configuration);
			_exampleUserId = "ABE3668C-F60F-426F-B20D-B764E1373352";
			_examplePostId = "0A1A0DAF-0040-4DB5-94C7-141A796C03F9";
			_exampleCommentId = "9B92C388-C51D-4EF5-997F-A40F3AFB7E9D";
			_invalidGuid = "InvalidGuid";
			_commentToAdd = new CommentToAdd()
			{
				WhenAdded = DateTime.Now,
				FromWho = new Guid(_exampleUserId),
				PostId = new Guid(_examplePostId),
				Text = "Example text"
			};
		}
		protected List<PostComment> GetTestPostData()
		{
			var database = new List<PostComment>()
			{
				new PostComment()
				{
				WhenAdded = DateTime.Now,
				Id = new Guid(_exampleCommentId),
				Text = "Example text",
				FromWho = new Guid("C48D3E36-2072-4A19-9305-FE5168BFB03D"),
				PostId = new Guid(_examplePostId)
				},
				new PostComment()
				{
				WhenAdded = DateTime.Now,
				Id = new Guid("A5A276EE-4936-4162-A2AC-880E525BD992"),
				Text = "Example text",
				FromWho = new Guid("603FD8C9-21CB-4348-8EE9-D768008F5FFE"),
				PostId = new Guid(_examplePostId)
				}
			};
			return database;
		}
		protected JsonPatchDocument<CommentToUpdate> GetJsonPatchDocument()
		{
			var jsonobject = new JsonPatchDocument<CommentToUpdate>
			{
				ContractResolver = new CamelCasePropertyNamesContractResolver()
			};
			jsonobject.Replace(d => d.Text, "Changed");
			return jsonobject;
		}
	}
}
