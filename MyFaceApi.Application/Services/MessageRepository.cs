using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyFaceApi.Api.DataAccess.Data;
using MyFaceApi.Api.DataAccess.Entities;
using MyFaceApi.Api.Repository.Helpers;
using MyFaceApi.Api.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFaceApi.Api.Repository.Repositories
{
	public class MessageRepository : IMessageRepository
	{
		private readonly IAppDbContext _appDbContext;
		private readonly ILogger<MessageRepository> _logger;

		public MessageRepository(IAppDbContext appDbContext,
			ILogger<MessageRepository> logger)
		{
			_appDbContext = appDbContext;
			_logger = logger;
		}
		public async Task<Message> AddMessageAsync(Message message)
		{
			_logger.LogDebug("Trying to add message {message}.", message);
			if (message is null)
			{
				throw new ArgumentNullException(nameof(message));
			}
			if (message.FromWho == Guid.Empty || message.ToWho == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(Guid));
			}
			try
			{
				var conversation = _appDbContext.Conversations.FirstOrDefault(
					x => x.FirstUser == message.ToWho && x.SecondUser == message.FromWho
					|| x.FirstUser == message.FromWho && x.SecondUser == message.ToWho);

				if (conversation is null)
				{
					conversation = new Conversation
					{
						FirstUser = message.ToWho,
						SecondUser = message.FromWho
					};
					await _appDbContext.Conversations.AddAsync(conversation);
				}
				message.ConversationId = conversation.Id;
				var addedMessage = await _appDbContext.Messages.AddAsync(message);
				await _appDbContext.SaveAsync();
				return addedMessage.Entity;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during adding the message.");
				throw;
			}
		}
		public async Task DeleteMessageAsync(Message message)
		{
			_logger.LogDebug("Trying to remove message: {message}.", message);
			if (message is null)
			{
				throw new ArgumentNullException(nameof(message));
			}
			try
			{
				_appDbContext.Messages.Remove(message);
				await _appDbContext.SaveAsync();
				_logger.LogDebug("Message {message} has been removed.", message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during removing the message.");
				throw;
			}
		}
		public IEnumerable<Message> GetLastUserMessages(Guid userId)
		{
			_logger.LogDebug("Trying to get user last messages: {userid}", userId);
			if (userId == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(userId));
			}
			try
			{
				var userConversations = _appDbContext.Conversations.Where(
					x => x.FirstUser == userId
					|| x.SecondUser == userId)
					.Select(x => x.Messages
					.OrderByDescending(x => x.When)
					.Take(1));

				List<Message> messagesToReturn = new List<Message>();
				foreach (var conv in userConversations)
				{
					messagesToReturn.AddRange(conv);
				}
				return messagesToReturn;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during getting user messages.");
				throw;
			}
		}
		public Message GetMessage(Guid messageId)
		{
			_logger.LogDebug("Trying to get the message: {messageId}", messageId);
			if (messageId == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(messageId));
			}
			try
			{
				var messageToReturn = _appDbContext.Messages.FirstOrDefault(m => m.Id == messageId);
				return messageToReturn;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during getting message.");
				throw;
			}
		}
		public async Task<List<Message>> GetUserMessagesWith(Guid userId, Guid friendId)
		{
			_logger.LogDebug($"Trying to get the users: {userId} and {friendId} messages");
			if (userId == Guid.Empty || friendId == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(Guid));
			}
			try
			{
				Conversation conversation = _appDbContext.Conversations
					.Include(nameof(_appDbContext.Messages))
					.FirstOrDefault(
					m => m.FirstUser == userId && m.SecondUser == friendId
					|| m.FirstUser == friendId && m.SecondUser == userId);

				if (conversation is null)
				{
					conversation = new Conversation
					{
						FirstUser = userId,
						SecondUser = friendId,
						Messages = new List<Message>()
					};
					_appDbContext.Conversations.Add(conversation);
					await _appDbContext.SaveAsync();
				}
				var collection = conversation.Messages.OrderByDescending(x => x.When).ToList();
				return collection;

			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during getting messages.");
				throw;
			}
		}
	}
}
