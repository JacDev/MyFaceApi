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

namespace MyFaceApi.Tests.UnitTests.UsersControllerTests
{
	public abstract class UsersControllerPreparation
	{
		protected readonly Mock<IUserRepository> _mockRepo;
		protected readonly Mock<ILogger<UsersController>> _loggerMock;
		protected readonly IMapper _mapper;
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
		}
		protected List<User> GetTestUserData()
		{
			var database = new List<User>
			{
				new User()
				{
					Id = new Guid("C48D3E36-2072-4A19-9305-FE5168BFB03D"),
					FirstName = "Mark",
					LastName = "Twain",
					ProfileImagePath = null,
				},
				new User()
				{
					Id = new Guid("24610263-CEE4-4231-97DF-904EE6437278"),
					FirstName = "Brad",
					LastName = "Pit"
				}
			};
			return database;
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
