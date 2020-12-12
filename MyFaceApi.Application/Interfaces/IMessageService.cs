using MyFaceApi.Api.Application.DtoModels.Message;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyFaceApi.Api.Application.Interfaces
{
	public interface IMessageService
	{
		List<MessageDto> GetUserMessagesWith(Guid userId, Guid friendId);
		Task<MessageDto> AddMessageAsync(Guid userId, MessageToAddDto message);
		Task DeleteMessageAsync(Guid messageid);
		MessageDto GetMessage(Guid messageId);
		IEnumerable<MessageDto> GetLastUserMessages(Guid userId);
	}
}
