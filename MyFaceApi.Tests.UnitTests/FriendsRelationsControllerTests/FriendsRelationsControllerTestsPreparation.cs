using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using MyFaceApi.Api.Controllers;
using System.Collections.Generic;
using MyFaceApi.Api.Application.Interfaces;
using Pagination.Helpers;
using MyFaceApi.Api.Application.DtoModels.FriendsRelation;

namespace MyFaceApi.Api.Tests.UnitTests.FriendsRelationsControllerTests
{
	public class FriendsRelationsControllerTestsPreparation
	{
		protected readonly Mock<IUserService> _mockUserService;
		protected readonly Mock<IFriendsRelationService> _mockRelationService;
		protected readonly Mock<ILogger<FriendsRelationsController>> _loggerMock;
		protected readonly IMapper _mapper;
		protected readonly IFixture _fixture;
		protected readonly PaginationParams _paginationsParams;

		protected FriendsRelationsControllerTestsPreparation()
		{
			//mocking Services
			_mockUserService = new Mock<IUserService>();
			_mockRelationService = new Mock<IFriendsRelationService>();
			//mocking logger
			_loggerMock = new Mock<ILogger<FriendsRelationsController>>();
			//mocking automapper
			var myProfile = new FriendsRelationProfile();
			var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
			_mapper = new Mapper(configuration);
			_fixture = new Fixture().Customize(new AutoMoqCustomization());
			_paginationsParams = new PaginationParams()
			{
				PageNumber = 0,
				PageSize = 10,
				Skip = 0
			};
		}
		protected List<FriendsRelationDto> GetTestRelationData()
		{
			var listToReturn = new List<FriendsRelationDto>
			{
				_fixture.Create<FriendsRelationDto>(),
				_fixture.Create<FriendsRelationDto>()
			};
			return listToReturn;
		}
	}
}
