using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using Moq;
using MyFaceApi.AutoMapperProfiles;
using MyFaceApi.Controllers;
using MyFaceApi.DataAccess.Entities;
using MyFaceApi.DataAccess.Enums;
using MyFaceApi.Models.NotificationModels;
using MyFaceApi.Repository.Interfaces;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;

namespace MyFaceApi.Tests.UnitTests.NotificationsControllerTests
{
	public class NotificationsControllerPreparation
	{
		protected readonly Mock<IUserRepository> _mockUserRepo;
		protected readonly Mock<INotificationRepository> _mockNotificationRepo;
		protected readonly Mock<ILogger<NotificationsController>> _loggerMock;
		protected readonly IMapper _mapper;
		protected readonly IFixture _fixture;

		protected NotificationsControllerPreparation()
		{
			//mocking repos
			_mockUserRepo = new Mock<IUserRepository>();
			_mockNotificationRepo = new Mock<INotificationRepository>();
			//mocking logger
			_loggerMock = new Mock<ILogger<NotificationsController>>();
			//mocking automapper
			var myProfile = new NotificationProfiles();
			var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
			_mapper = new Mapper(configuration);
			_fixture = new Fixture().Customize(new AutoMoqCustomization());
		}
		protected List<Notification> GetTestNotificationData()
		{
			var listToReturn = new List<Notification>
			{
				new Notification
					{
						ToWhoId = new Guid(ConstIds.ExampleUserId),
						Id = new Guid(ConstIds.ExampleNotificationId),
						WhenAdded = DateTime.Now,
						EventId = new Guid(ConstIds.ExampleEventId),
						FromWho = new Guid(ConstIds.ExampleFromWhoId),
						HasSeen = false,
						NotificationType = NotificationType.Comment,
					},
				new Notification
					{
						ToWhoId = new Guid(ConstIds.ExampleUserId),
						Id = new Guid("CEE52E51-F4A4-4AC8-8AB7-1ACF73AA863F"),
						WhenAdded = DateTime.Now,
						EventId = new Guid("56F23FBA-66F8-44A2-BAFD-3C69520781CF"),
						FromWho = new Guid(ConstIds.ExampleFromWhoId),
						HasSeen = false,
						NotificationType = NotificationType.FriendRequiest,
					}
			};
			return listToReturn;
		}

		protected JsonPatchDocument<NotificationToUpdate> GetJsonPatchDocument()
		{
			var jsonobject = new JsonPatchDocument<NotificationToUpdate>
			{
				ContractResolver = new CamelCasePropertyNamesContractResolver()
			};
			jsonobject.Replace(d => d.HasSeen, true);
			return jsonobject;
		}
	}
}
