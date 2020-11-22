﻿using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using MyFaceApi.AutoMapperProfiles;
using MyFaceApi.Controllers;
using MyFaceApi.DataAccess.Entities;
using MyFaceApi.Repository.Interfaces;
using System.Collections.Generic;

namespace MyFaceApi.Tests.UnitTests.FriendsRelationsControllerTests
{
	public class FriendsRelationsControllerTestsPreparation
	{
		protected readonly Mock<IUserRepository> _mockUserRepo;
		protected readonly Mock<IFriendsRelationRepository> _mockRelationRepo;
		protected readonly Mock<ILogger<FriendsRelationsController>> _loggerMock;
		protected readonly IMapper _mapper;
		protected readonly IFixture _fixture;
		protected FriendsRelationsControllerTestsPreparation()
		{
			//mocking repos
			_mockUserRepo = new Mock<IUserRepository>();
			_mockRelationRepo = new Mock<IFriendsRelationRepository>();
			//mocking logger
			_loggerMock = new Mock<ILogger<FriendsRelationsController>>();
			//mocking automapper
			var myProfile = new FriendsRelationProfiles();
			var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
			_mapper = new Mapper(configuration);
			_fixture = new Fixture().Customize(new AutoMoqCustomization());
		}
		protected List<FriendsRelation> GetTestRelationData()
		{
			var listToReturn = new List<FriendsRelation>
			{
				_fixture.Create<FriendsRelation>(),
				_fixture.Create<FriendsRelation>()
			};
			return listToReturn;
		}
	}
}