using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyFaceApi.Api.DataAccess.Entities;
using MyFaceApi.Api.Models.NotificationModels;
using MyFaceApi.Api.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyFaceApi.Api.Controllers
{
	[Route("api/users/{userId}/notifications")]
	[ApiController]
	public class NotificationsController : ControllerBase
	{
		private readonly ILogger<NotificationsController> _logger;
		private readonly INotificationRepository _notificationRepository;
		private readonly IUserRepository _userRepository;
		private readonly IMapper _mapper;

		public NotificationsController(ILogger<NotificationsController> logger,
			INotificationRepository notificationRepository,
			IMapper mapper, 
			IUserRepository userRepository)
		{
			_logger = logger;
			_notificationRepository = notificationRepository;
			_mapper = mapper;
			_userRepository = userRepository;
			_logger.LogTrace("NotificationController created");
		}
		/// <summary>
		/// Return the found notification
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="notificationId">Notification guid as a string </param>
		/// <returns>Found notification</returns>
		/// <response code="200"> Returns the found notification</response>
		/// <response code="400"> If parameter is not a valid guid</response>    
		/// <response code="404"> If notification not found</response>   
		/// <response code="500"> If internal error occured</response>
		[HttpGet("{notificationId}", Name = "GetNotification")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<Post>> GetNotification(string userId, string notificationId)
		{
			try
			{
				if (Guid.TryParse(userId, out Guid gUserId) && Guid.TryParse(notificationId, out Guid gNotificationId))
				{
					if (await _userRepository.CheckIfUserExists(gUserId))
					{
						Notification notificationToReturn = _notificationRepository.GetNotification(gNotificationId);
						if (notificationToReturn is null)
						{
							return NotFound($"Notification: {notificationId} not found.");
						}
						else
						{
							return Ok(notificationToReturn);
						}
					}
					else
					{
						return NotFound($"User: {userId} not found.");
					}
				}
				else
				{
					return BadRequest($"User id: {userId} or  notification id: {notificationId} is not valid Guid.");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during getting the notification. Notification id: {notificationId}", notificationId);
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}
		/// <summary>
		/// Return the found user notifications
		/// </summary>
		/// <param name="userId">User guid as a string </param>
		/// <returns>Found user notifications</returns>
		/// <response code="200"> Returns the found user notifications</response>
		/// <response code="400"> If parameter is not a valid guid</response>    
		/// <response code="404"> If user or notifications not found</response>   
		/// <response code="500"> If internal error occured</response>
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<List<Post>>> GetNotifications(string userId)
		{
			try
			{
				if (Guid.TryParse(userId, out Guid gUserId))
				{
					if (await _userRepository.CheckIfUserExists(gUserId))
					{
						List<Notification> notificationsToReturn = _notificationRepository.GetUserNotifications(gUserId);
						if(notificationsToReturn is null)
						{
							return NotFound($"The user: {userId} notificatios not found.");
						}
						else
						{
							return Ok(notificationsToReturn);
						}
					}
					else
					{
						return NotFound($"User: {userId} not found.");
					}
				}
				else
				{
					return BadRequest($"{userId} is not valid Guid.");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during getting the user notifications. User id: {user}", userId);
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}
		/// <summary>
		/// Add notification to database
		/// </summary>
		/// <param name="userId">User guid as a string </param>
		/// <param name="notificationToAdd"></param>
		/// <returns>Added reaction</returns>
		/// <response code="201"> Return created notification</response>
		/// <response code="400"> If parameter is not a valid guid</response>    
		/// <response code="404"> If user not found</response>   
		/// <response code="500"> If internal error occured</response>
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<NotificationToAdd>> AddNotification(string userId,  [FromBody] NotificationToAdd notificationToAdd)
		{
			try
			{
				if (Guid.TryParse(userId, out Guid gUserId))
				{
					if (await _userRepository.CheckIfUserExists(gUserId))
					{
						Notification notificationEntity = _mapper.Map<Notification>(notificationToAdd);
						notificationEntity.ToWhoId = gUserId;
						notificationEntity = await _notificationRepository.AddNotificationAsync(notificationEntity);

						return CreatedAtRoute("GetNotification",
							new
							{
								userId,
								notificationId = notificationEntity.Id
							},
							notificationToAdd);
					}
					else
					{
						return NotFound($"User: {userId} not found.");
					}
				}
				else
				{
					return BadRequest($"{userId} is not valid Guid.");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during adding the notification. User id: {userId}", userId);
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}
		/// <summary>
		/// Update notification in the database
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="notificationId"></param>
		/// <param name="patchDocument"></param>
		/// <returns>
		/// Status 204 no content if the notification has been updated
		/// </returns>
		/// <response code="204"> No content if the notification has been updated</response>
		/// <response code="400"> If the notificationId is not valid guid</response>    
		/// <response code="404"> If the notification not found</response>
		/// <response code="500"> If internal error occured</response>
		[HttpPatch("{notificationId}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult> PartiallyUpdateNotification(string userId, string notificationId, JsonPatchDocument<NotificationToUpdate> patchDocument)
		{
			try
			{
				if (Guid.TryParse(userId, out Guid gUserId) && Guid.TryParse(notificationId, out Guid gNotificationId))
				{
					if (await _userRepository.CheckIfUserExists(gUserId))
					{
						Notification notificationFromRepo = _notificationRepository.GetNotification(gNotificationId);
						if (notificationFromRepo is null)
						{
							return NotFound($"Notification: {notificationId} not found.");
						}
						NotificationToUpdate notificationToPatch = _mapper.Map<NotificationToUpdate>(notificationFromRepo);
						patchDocument.ApplyTo(notificationToPatch, ModelState);

						if (!TryValidateModel(notificationToPatch))
						{
							return ValidationProblem(ModelState);
						}

						_mapper.Map(notificationToPatch, notificationFromRepo);

						await _notificationRepository.UpdateNotificationAsync(notificationFromRepo);
						return NoContent();
					}
					else
					{
						return NotFound($"User: {userId} not found.");
					}
				}
				else
				{
					return BadRequest($"User id: {userId} or notification id: {notificationId} is not valid Guid.");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during updating the notification. Notification id: {notificationId}", notificationId);
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}
		/// <summary>
		/// Remove the notification from the database
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="notificationId"></param>
		/// <returns>
		/// Status 204 no content if the notification has been removed
		/// </returns>
		/// <response code="204"> No content if the notification has been removed</response>
		/// <response code="400"> If the notificationId is not valid guid</response>    
		/// <response code="404"> If the notification not found</response>
		/// <response code="500"> If internal error occured</response>
		[HttpDelete("{notificationId}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult> DeleteNotification(string userId, string notificationId)
		{
			try
			{
				if (Guid.TryParse(userId, out Guid gUserId) && Guid.TryParse(notificationId, out Guid gNotificationId))
				{
					if (await _userRepository.CheckIfUserExists(gUserId))
					{
						Notification notificationToRemove = _notificationRepository.GetNotification(gNotificationId);
						if (notificationToRemove is null)
						{
							return NotFound($"Notification: {notificationId} not found.");
						}
						await _notificationRepository.DeleteNotificationAsync(notificationToRemove);
						return NoContent();
					}
					else
					{
						return NotFound($"User: {userId} not found.");
					}
				}
				else
				{
					return BadRequest($"User id: {userId} or notification id: {notificationId} is not valid Guid.");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during removing the notification. Notification id: {notificationId}", notificationId);
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}
	}
}
