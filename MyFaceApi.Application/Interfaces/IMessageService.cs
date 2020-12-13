using MyFaceApi.Api.Application.DtoModels.Message;
using MyFaceApi.Api.Application.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyFaceApi.Api.Application.Interfaces
{
	public interface IMessageService
	{
		PagedList<MessageDto> GetUserMessagesWith(Guid userId, Guid friendId, PaginationParams paginationParams);
		Task<MessageDto> AddMessageAsync(Guid userId, MessageToAddDto message);
		Task DeleteMessageAsync(Guid messageid);
		MessageDto GetMessage(Guid messageId);
		IEnumerable<MessageDto> GetLastUserMessages(Guid userId);
	}
}
