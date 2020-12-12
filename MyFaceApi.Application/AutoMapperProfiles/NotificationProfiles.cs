using AutoMapper;
using MyFaceApi.Api.Application.DtoModels.Notification;
using MyFaceApi.Api.Domain.Entities;

namespace MyFaceApi.AutoMapperProfiles
{
	public class NotificationProfiles : Profile
	{
		public NotificationProfiles()
		{
			CreateMap<NotificationToAddDto, Notification>();
			CreateMap<Notification, NotificationDto>();
		}
	}
}
