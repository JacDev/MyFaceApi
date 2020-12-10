using MyFaceApi.Api.DataAccess.Entities;
using MyFaceApi.Api.Repository.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyFaceApi.Api.Repository.Interfaces
{
	public interface IMessageRepository
	{
		Task<List<Message>> GetUserMessagesWith(Guid userId, Guid friendId);
		Task<Message> AddMessageAsync(Message message);
		Task DeleteMessageAsync(Message messageid);
		Message GetMessage(Guid messageId);
		IEnumerable<Message> GetLastUserMessages(Guid userId);
	}
}
