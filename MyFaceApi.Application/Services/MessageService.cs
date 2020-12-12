using AutoMapper;
using Microsoft.Extensions.Logging;
using MyFaceApi.Api.Application.DtoModels.Message;
using MyFaceApi.Api.Application.Interfaces;
using MyFaceApi.Api.Domain.Entities;
using MyFaceApi.Api.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFaceApi.Api.Application.Services
{
	public class MessageService : IMessageService
	{
		private readonly IRepository<Message> _messageRepository;
		private readonly ILogger<MessageService> _logger;
		private readonly IMapper _mapper;
		public MessageService(IRepository<Message> messageRepository,
			ILogger<MessageService> logger,
			IMapper mapper)
		{
			_messageRepository = messageRepository;
			_logger = logger;
			_mapper = mapper;
		}

		public async Task<MessageDto> AddMessageAsync(Guid userId, MessageToAddDto message)
		{
			_logger.LogDebug("Trying to add the message {message}.", message);
			if (message is null)
			{
				throw new ArgumentNullException(nameof(message));
			}
			if (message.FromWho == Guid.Empty || userId == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(Guid));
			}
			try
			{
				Message messageToAdd = _mapper.Map<Message>(message);
				messageToAdd.ToWho = userId;
				Message addedMessage = await _messageRepository.AddAsync(messageToAdd);
				await _messageRepository.SaveAsync();
				return _mapper.Map<MessageDto>(addedMessage);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during adding the message.");
				throw;
			}
		}
		public async Task DeleteMessageAsync(Guid messageid)
		{
			_logger.LogDebug("Trying to remove message: {messageid}.", messageid);
			if (messageid ==Guid.Empty)
			{
				throw new ArgumentNullException(nameof(messageid));
			}
			try
			{
				var messageToRemove = _messageRepository.GetById(messageid);
				if (messageToRemove != null)
				{
					_messageRepository.Remove(messageToRemove);
					await _messageRepository.SaveAsync();
					_logger.LogDebug("Message {message} has been removed.", messageToRemove);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during removing the message.");
				throw;
			}
		}
		public IEnumerable<MessageDto> GetLastUserMessages(Guid userId)
		{
			//_logger.LogDebug("Trying to get user last messages: {userid}", userId);
			//if (userId == Guid.Empty)
			//{
			//	throw new ArgumentNullException(nameof(userId));
			//}
			//try
			//{
			//	var allMessages = _messageRepository.Get(x => (x.FromWho == userId || x.ToWho == userId), x => x.OrderByDescending(x => x.When))
			//		.GroupBy(x => new { x.FromWho, x.ToWho });

			//	var userConversations = _appDbContext.Conversations.Where
			//		.Select(x => x.Messages

			//		.Take(1));

			//	List<Message> messagesToReturn = new List<Message>();
			//	foreach (var conv in userConversations)
			//	{
			//		messagesToReturn.AddRange(conv);
			//	}
			//	return messagesToReturn;
			//}
			//catch (Exception ex)
			//{
			//	_logger.LogError(ex, "Error occured during getting user messages.");
			//	throw;
			//}
			throw new NotImplementedException();
		}
		public MessageDto GetMessage(Guid messageId)
		{
			_logger.LogDebug("Trying to get the message: {messageId}", messageId);
			if (messageId == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(messageId));
			}
			try
			{
				var message = _messageRepository.GetById(messageId);		
				return _mapper.Map<MessageDto>(message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during getting message.");
				throw;
			}
		}
		public List<MessageDto> GetUserMessagesWith(Guid userId, Guid friendId)
		{
			_logger.LogDebug($"Trying to get the users: {userId} and {friendId} messages");
			if (userId == Guid.Empty || friendId == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(Guid));
			}
			try
			{
				List<Message> messagesFromRepo = _messageRepository.Get(
					m => m.ToWho == userId && m.FromWho == friendId || m.ToWho == friendId && m.FromWho == userId,
					x=>x.OrderByDescending(x => x.When)).ToList();

				return _mapper.Map<List<MessageDto>>(messagesFromRepo);

			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during getting messages.");
				throw;
			}
		}
	}
}
