using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using Moq;
using MyFaceApi.AutoMapperProfiles;
using MyFaceApi.Controllers;
using MyFaceApi.Entities;
using MyFaceApi.Models;
using MyFaceApi.Repository.Interfaceses;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyFaceApi.Tests.UnitTests.UsersControllerTests
{
	public abstract class UsersControllerPreparation
	{
		protected readonly Mock<IUserRepository> _mockRepo;
		protected readonly Mock<ILogger<UsersController>> _loggerMock;
		protected readonly IMapper _mapper;
		protected readonly BasicUserData _userToAdd;
		protected UsersControllerPreparation()
		{
			//mocking UserRepo
			_mockRepo = new Mock<IUserRepository>();
			//mocking logger
			_loggerMock = new Mock<ILogger<UsersController>>();
			//mocking automapper
			var myProfile = new UserProfiles();
			var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
			_mapper = new Mapper(configuration);
			_userToAdd = _mapper.Map<BasicUserData>(GetTestUserData());
		}
		protected User GetTestUserData()
		{

			return new User()
			{
				Id = new Guid(ConstIds.ExampleUserId),
				FirstName = "Mark",
				LastName = "Twain",
				ProfileImagePath = null,
			};
		}
		protected JsonPatchDocument<BasicUserData> GetJsonPatchDocument()
		{
			var jsonobject = new JsonPatchDocument<BasicUserData>
			{
				ContractResolver = new CamelCasePropertyNamesContractResolver()
			};
			jsonobject.Replace(d => d.FirstName, "Changed");
			return jsonobject;
		}
	}
}
