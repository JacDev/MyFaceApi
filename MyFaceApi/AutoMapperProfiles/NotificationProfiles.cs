using AutoMapper;
using MyFaceApi.Api.DataAccess.Entities;
using MyFaceApi.Api.Models.NotificationModels;

namespace MyFaceApi.AutoMapperProfiles
{
	public class NotificationProfiles : Profile
	{
		public NotificationProfiles()
		{
			CreateMap<NotificationToAdd, Notification>();
			CreateMap<Notification, NotificationToAdd>();
			CreateMap<NotificationToUpdate, Notification>();
			CreateMap<Notification, NotificationToUpdate>();
		}
	}
}
