using AutoFixture.AutoMoq;
using AutoFixture;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using Moq;
using MyFaceApi.AutoMapperProfiles;
using MyFaceApi.Api.Controllers;
using MyFaceApi.Api.DataAccess.Entities;
using MyFaceApi.Api.DataAccess.ModelsBasicInfo;
using MyFaceApi.Api.Repository.Interfaces;
using Newtonsoft.Json.Serialization;

namespace MyFaceApi.Tests.UnitTests.UsersControllerTests
{
	public abstract class UsersControllerPreparation
	{
		protected readonly Mock<IUserRepository> _mockUserRepo;
		protected readonly Mock<ILogger<UsersController>> _loggerMock;
		protected readonly IMapper _mapper;
		protected readonly IFixture _fixture;
		protected UsersControllerPreparation()
		{
			//mocking UserRepo
			_mockUserRepo = new Mock<IUserRepository>();
			//mocking logger
			_loggerMock = new Mock<ILogger<UsersController>>();
			//mocking automapper
			var myProfile = new UserProfiles();
			var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
			_mapper = new Mapper(configuration);
			_fixture = new Fixture().Customize(new AutoMoqCustomization());
		}
		protected User GetTestUserData()
		{
			return _fixture.Create<User>();
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
