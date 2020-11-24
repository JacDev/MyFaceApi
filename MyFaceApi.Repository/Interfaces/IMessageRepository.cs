using MyFaceApi.DataAccess.Entities;
using MyFaceApi.Repository.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyFaceApi.Repository.Interfaces
{
	public interface IMessageRepository
	{
		PagedList<Message> GetUserMessagesWith(Guid userId, Guid friendId, PaginationParams paginationParams);
		Task<Message> AddMessageAsync(Message message);
		Task DeleteMessageAsync(Message messageid);
		Message GetMessage(Guid messageId);
		IEnumerable<Message> GetLastUserMessages(Guid userId);
	}
}
