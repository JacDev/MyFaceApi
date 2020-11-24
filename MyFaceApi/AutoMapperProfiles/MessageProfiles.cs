using AutoMapper;
using MyFaceApi.DataAccess.Entities;
using MyFaceApi.Models.MessageModels;

namespace MyFaceApi.Api.AutoMapperProfiles
{
	public class MessageProfiles : Profile
	{
		public MessageProfiles()
		{
			CreateMap<MessageToAdd, Message>();
			CreateMap<Message, MessageToAdd>();


		}
	}
}
