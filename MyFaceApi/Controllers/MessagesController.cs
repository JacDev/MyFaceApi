using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyFaceApi.Api.Application.DtoModels;
using MyFaceApi.Api.Application.DtoModels.Message;
using MyFaceApi.Api.Application.Helpers;
using MyFaceApi.Api.Application.Interfaces;
using MyFaceApi.Api.Extensions;
using System;
using System.Threading.Tasks;

namespace MyFaceApi.Api.Controllers
{
	[Route("api/users/{userId}/messages")]
	[ApiController]
	public class MessagesController : ControllerBase
	{
		private readonly ILogger<MessagesController> _logger;
		private readonly IMessageService _messageService;
		private readonly IUserService _userService;
		public MessagesController(ILogger<MessagesController> logger,
			IMessageService messageService,
			IUserService userService)
		{
			_logger = logger;
			_messageService = messageService;
			_userService = userService;
			_logger.LogTrace("MessagesController created");
		}

		[HttpGet("{messageId}", Name = "GetMessage")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public ActionResult<MessageDto> GetMessage(string messageId)
		{
			if (Guid.TryParse(messageId, out Guid gMessageId))
			{
				try
				{
					MessageDto messageToReturn = _messageService.GetMessage(gMessageId);
					return Ok(messageToReturn);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error occured during getting the message. Message id: {id}", messageId);
					return StatusCode(StatusCodes.Status500InternalServerError);
				}
			}
			else
			{
				return BadRequest($"{messageId} is not valid guid.");
			}
		}
		[HttpGet("with/{friendId}", Name = "GetMessagesWith")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<CollectionWithPaginationData<MessageDto>>> GetMessagesWith(string userId, string friendId, [FromQuery] PaginationParams paginationParams)
		{
			if (Guid.TryParse(userId, out Guid gUserId) && Guid.TryParse(friendId, out Guid gFriendId))
			{
				try
				{
					if (await _userService.CheckIfUserExists(gUserId) && await _userService.CheckIfUserExists(gFriendId))
					{
						PagedList<MessageDto> messagesToReturn = _messageService.GetUserMessagesWith(gUserId, gFriendId, paginationParams);
						return Ok(this.CreateCollectionWithPagination(messagesToReturn, paginationParams, "GetMessagesWith"));

					}
					else
					{
						return NotFound($"User: {userId} or user: {friendId} doesnt exist.");
					}
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error occured during getting the messages. User id: {userId} and friend id: {friendId}", userId, friendId);
					return StatusCode(StatusCodes.Status500InternalServerError);
				}
			}
			else
			{
				return BadRequest($"{userId} or user: {friendId} is not valid guid.");
			}
		}
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<MessageDto>> AddMessage(string userId, MessageToAddDto messageToAdd)
		{
			try
			{
				if (Guid.TryParse(userId, out Guid gUserId))
				{
					if (await _userService.CheckIfUserExists(gUserId))
					{
						MessageDto addedMessage = await _messageService.AddMessageAsync(gUserId, messageToAdd);
						return CreatedAtRoute("GetMessage",
							new
							{
								userId,
								messageId = addedMessage.Id
							},
							addedMessage);
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
					await _messageService.DeleteMessageAsync(gMessageId);
					return NoContent();
				}
				else
				{
					return BadRequest($"{messageId} is not valid guid.");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during removing the message. Message id: {id}", messageId);
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}
	}
}
