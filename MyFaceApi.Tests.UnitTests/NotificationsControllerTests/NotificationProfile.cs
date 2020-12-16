using AutoMapper;
using MyFaceApi.Api.Application.DtoModels.Notification;

namespace MyFaceApi.Api.Tests.UnitTests.NotificationsControllerTests
{
	class NotificationProfile : Profile
	{
		public NotificationProfile()
		{
			CreateMap<NotificationToAddDto, NotificationDto>();
		}
	}
}
