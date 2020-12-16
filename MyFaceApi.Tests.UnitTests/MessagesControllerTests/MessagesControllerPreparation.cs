using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using MyFaceApi.Api.Application.Interfaces;
using MyFaceApi.Api.Controllers;

namespace MyFaceApi.Api.Tests.UnitTests.MessagesControllerTests
{
	public class MessagesControllerPreparation
	{
		protected readonly Mock<IUserService> _mockUserService;
		protected readonly Mock<IMessageService> _mockMessagesService;
		protected readonly Mock<ILogger<MessagesController>> _loggerMock;
		protected readonly IMapper _mapper;
		protected readonly IFixture _fixture;
		public MessagesControllerPreparation()
		{
			//mocking Services
			_mockUserService = new Mock<IUserService>();
			_mockMessagesService = new Mock<IMessageService>();
			//mocking logger
			_loggerMock = new Mock<ILogger<MessagesController>>();
			//mocking automapper
			var myProfile = new MessageProfile();
			var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
			_mapper = new Mapper(configuration);
			_fixture = new Fixture().Customize(new AutoMoqCustomization());
		}
	}
}
