using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using Moq;
using MyFaceApi.AutoMapperProfiles;
using MyFaceApi.Api.Controllers;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using MyFaceApi.Api.Application.Interfaces;
using MyFaceApi.Api.Application.DtoModels.Notification;
using MyFaceApi.Api.Domain.Enums;

namespace MyFaceApi.Tests.UnitTests.NotificationsControllerTests
{
	public class NotificationsControllerPreparation
	{
		protected readonly Mock<IUserService> _mockUserService;
		protected readonly Mock<INotificationService> _mockNotificationService;
		protected readonly Mock<ILogger<NotificationsController>> _loggerMock;
		protected readonly IMapper _mapper;
		protected readonly IFixture _fixture;

		protected NotificationsControllerPreparation()
		{
			//mocking Services
			_mockUserService = new Mock<IUserService>();
			_mockNotificationService = new Mock<INotificationService>();
			//mocking logger
			_loggerMock = new Mock<ILogger<NotificationsController>>();
			//mocking automapper
			var myProfile = new NotificationProfiles();
			var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
			_mapper = new Mapper(configuration);
			_fixture = new Fixture().Customize(new AutoMoqCustomization());
		}
		protected List<NotificationDto> GetTestNotificationData()
		{
			var listToReturn = new List<NotificationDto>
			{
				new NotificationDto
					{
						Id = new Guid(ConstIds.ExampleNotificationId),
						WhenAdded = DateTime.Now,
						EventId = new Guid(ConstIds.ExampleEventId),
						FromWho = new Guid(ConstIds.ExampleFromWhoId),
						HasSeen = false,
						NotificationType = NotificationType.Comment
					},
				new NotificationDto
					{
						Id = new Guid("CEE52E51-F4A4-4AC8-8AB7-1ACF73AA863F"),
						WhenAdded = DateTime.Now,
						EventId = new Guid("56F23FBA-66F8-44A2-BAFD-3C69520781CF"),
						FromWho = new Guid(ConstIds.ExampleFromWhoId),
						HasSeen = false,
						NotificationType = NotificationType.FriendRequiest
					}
			};
			return listToReturn;
		}

		protected JsonPatchDocument<NotificationToUpdateDto> GetJsonPatchDocument()
		{
			var jsonobject = new JsonPatchDocument<NotificationToUpdateDto>
			{
				ContractResolver = new CamelCasePropertyNamesContractResolver()
			};
			jsonobject.Replace(d => d.HasSeen, true);
			return jsonobject;
		}
	}
}
