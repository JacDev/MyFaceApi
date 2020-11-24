using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyFaceApi.Api.DataAccess.Entities;
using MyFaceApi.Api.Repository.Helpers;
using MyFaceApi.Api.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using MyFaceApi.Api.Extensions;
using MyFaceApi.Api.Helpers;
using MyFaceApi.Api.Models.MessageModels;

namespace MyFaceApi.Api.Controllers
{
	[Route("api/users/{userId}/messages")]
	[ApiController]
	public class MessagesController : ControllerBase
	{
		private readonly ILogger<MessagesController> _logger;
		private readonly IMessageRepository _messageRepository;
		private readonly IUserRepository _userRepository;
		private readonly IMapper _mapper;
		public MessagesController(ILogger<MessagesController> logger,
			IMessageRepository messageRepository,
			IUserRepository userRepository,
			IMapper mapper)
		{
			_logger = logger;
			_messageRepository = messageRepository;
			_userRepository = userRepository;
			_mapper = mapper;
			_logger.LogTrace("MessagesController created");
		}
		[HttpGet("populate")]
		public async Task<ActionResult<List<Message>>> PopulateDb()
		{
			var _fixture = new Fixture().Customize(new AutoMoqCustomization());
			var messages = new List<Message>();
			var users = new List<User>();
			_fixture.AddManyTo(users, 2);
			await _userRepository.AddUserAcync(users[0]);
			await _userRepository.AddUserAcync(users[1]);
			_fixture.AddManyTo(messages, 15);
			for(int i = 0;i<10;++i)
			{
				messages[i].FromWho = users[0].Id;
				messages[i].ToWho = users[1].Id;
				await _messageRepository.AddMessageAsync(messages[i]);
			}
			for (int i = 10; i < messages.Count; ++i)
			{
				messages[i].FromWho = users[1].Id;
				messages[i].ToWho = users[0].Id;
				await _messageRepository.AddMessageAsync(messages[i]);
			}
			return messages;		
		}
		[HttpGet("{messageId}", Name = "GetMessage")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public ActionResult<Message> GetMessage(string messageId)
		{
			try
			{
				if (Guid.TryParse(messageId, out Guid gMessageId))
				{
					Message messageToReturn = _messageRepository.GetMessage(gMessageId);
					return Ok(messageToReturn);
				}
				else
				{
					return BadRequest($"{messageId} is not valid guid.");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during getting the message. Message id: {messageId}", messageId);
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}
		[HttpGet("with/{friendId}", Name = "GetMessages")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public ActionResult<PagedList<Message>> GetMessagesWith(string userId, string friendId, [FromQuery] PaginationParams paginationParams)
		{
			try
			{
				if (Guid.TryParse(userId, out Guid gUserId) && Guid.TryParse(friendId, out Guid gFriendId))
				{
					if (_userRepository.CheckIfUserExists(gUserId) && _userRepository.CheckIfUserExists(gFriendId))
					{
						PagedList<Message> messagesToReturn = _messageRepository.GetUserMessagesWith(gUserId, gFriendId, paginationParams);
						if (messagesToReturn != null)
						{
							messagesToReturn.PreviousPageLink = messagesToReturn.HasPrevious ?
								this.CreateMessagesResourceUriWithPaginationParams(paginationParams, ResourceUriType.PreviousPage, "GetMessages") : null;

							messagesToReturn.NextPageLink = messagesToReturn.HasNext ?
								this.CreateMessagesResourceUriWithPaginationParams(paginationParams, ResourceUriType.NextPage, "GetMessages") : null;

							var paginationMetadata = PaginationHelper.CreatePaginationMetadata<Message>(messagesToReturn);
							Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));
						}
						return Ok(messagesToReturn);
					}
					else
					{
						return NotFound($"User: {userId} or user: {friendId} doesnt exist.");
					}
				}
				else
				{
					return BadRequest($"{userId} or user: {friendId} is not valid guid.");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during getting the messages. User id: {userId} and friend id: {friendId}", userId, friendId);
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<Message>> AddMessage(string userId, MessageToAdd messageToAdd)
		{
			try
			{
				if (Guid.TryParse(userId, out Guid gUserId))
				{
					if (_userRepository.CheckIfUserExists(gUserId))
					{
						Message messageEntity = _mapper.Map<Message>(messageToAdd);

						messageEntity.ToWho = gUserId;
						messageEntity = await _messageRepository.AddMessageAsync(messageEntity);

						return CreatedAtRoute("GetMessage",
							new { userId, messageId = messageEntity.Id },
							messageEntity);
					}
					else
					{
						return NotFound($"User: {userId} not found.");
					}
				}
				else
				{
					return BadRequest($"{userId} is not valid guid.");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during adding the user message. User id: {user}", userId);
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}
		[HttpDelete("{messageId}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult> DeleteMessage(string messageId)
		{
			try
			{
				if (Guid.TryParse(messageId, out Guid gMessageId))
				{
					Message messageFromRepo = _messageRepository.GetMessage(gMessageId);
					if (messageFromRepo == null)
					{
						return NotFound();
					}
					await _messageRepository.DeleteMessageAsync(messageFromRepo);
					return NoContent();
				}
				else
				{
					return BadRequest($"{messageId} is not valid guid.");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during removing the message. Message id: {messageId}", messageId);
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}


	}
}
