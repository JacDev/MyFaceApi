using AutoMapper;
using MyFaceApi.Api.Application.DtoModels.Message;

namespace MyFaceApi.Api.Tests.UnitTests.MessagesControllerTests
{
	public class MessageProfile : Profile
	{
		public MessageProfile()
		{
			CreateMap<MessageToAddDto, MessageDto>();
		}
	}
}
