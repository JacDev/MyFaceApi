using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using MyFaceApi.Api.AutoMapperProfiles;
using MyFaceApi.Api.Controllers;
using MyFaceApi.AutoMapperProfiles;
using MyFaceApi.Api.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyFaceApi.Tests.UnitTests.MessagesControllerTests
{
	public class MessagesControllerPreparation
	{
		protected readonly Mock<IUserRepository> _mockUserRepo;
		protected readonly Mock<IMessageRepository> _mockMessagesRepo;
		protected readonly Mock<ILogger<MessagesController>> _loggerMock;
		protected readonly IMapper _mapper;
		protected readonly IFixture _fixture;
		public MessagesControllerPreparation()
		{
			//mocking repos
			_mockUserRepo = new Mock<IUserRepository>();
			_mockMessagesRepo = new Mock<IMessageRepository>();
			//mocking logger
			_loggerMock = new Mock<ILogger<MessagesController>>();
			//mocking automapper
			var myProfile = new MessageProfiles();
			var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
			_mapper = new Mapper(configuration);
			_fixture = new Fixture().Customize(new AutoMoqCustomization());
		}
	}
}
