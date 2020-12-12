using AutoMapper;
using MyFaceApi.Api.DataAccess.Entities;
using MyFaceApi.Api.Models.MessageModels;

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
