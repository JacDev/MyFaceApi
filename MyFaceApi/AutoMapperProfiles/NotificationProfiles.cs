using AutoMapper;
using MyFaceApi.DataAccess.Entities;
using MyFaceApi.Models.NotificationModels;

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
