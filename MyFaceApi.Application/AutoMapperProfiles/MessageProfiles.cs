﻿using AutoMapper;
using MyFaceApi.Api.Application.DtoModels.Message;
using MyFaceApi.Api.Domain.Entities;

namespace MyFaceApi.Api.Application.AutoMapperProfiles
{
	public class MessageProfiles : Profile
	{
		public MessageProfiles()
		{
			CreateMap<MessageToAddDto, Message>();
			CreateMap<Message, MessageDto>();
		}
	}
}
