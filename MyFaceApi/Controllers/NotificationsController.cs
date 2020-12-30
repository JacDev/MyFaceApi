using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyFaceApi.Api.Application.DtoModels.Notification;
using MyFaceApi.Api.Application.Interfaces;
using Pagination.DtoModels;
using Pagination.Extensions;
using Pagination.Helpers;
using System;
using System.Threading.Tasks;

namespace MyFaceApi.Api.Controllers
{
	[Route("api/users/{userId}/notifications")]
	[ApiController]
	public class NotificationsController : ControllerBase
	{
		private readonly ILogger<NotificationsController> _logger;
		private readonly INotificationService _notificationService;
		private readonly IUserService _userService;

		public NotificationsController(ILogger<NotificationsController> logger,
			INotificationService notificationService,
			IUserService userService)
		{
			_logger = logger;
			_notificationService = notificationService;
			_userService = userService;
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
		public async Task<ActionResult<NotificationDto>> GetNotification(string userId, string notificationId)
		{
			if (Guid.TryParse(userId, out Guid gUserId) && Guid.TryParse(notificationId, out Guid gNotificationId))
			{
				try
				{
					if (await _userService.CheckIfUserExists(gUserId))
					{
						NotificationDto notificationToReturn = _notificationService.GetNotification(gNotificationId);
						return Ok(notificationToReturn);
					}
					else
					{
						return NotFound($"User: {userId} not found.");
					}
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error occured during getting the notification. Notification id: {id}", notificationId);
					return StatusCode(StatusCodes.Status500InternalServerError);
				}
			}
			else
			{
				return BadRequest($"User id: {userId} or notification id: {notificationId} is not valid guid.");
			}
		}

		/// <summary>
		/// Return the found user notifications
		/// </summary>
		/// <param name="userId">User guid as a string </param>
		/// <param name="paginationParams"></param>
		/// <param name="fromWhoId"></param>
		/// <param name="notificationType"></param>
		/// <returns>Found user notifications</returns>
		/// <response code="200"> Returns the found user notifications</response>
		/// <response code="400"> If parameter is not a valid guid</response>    
		/// <response code="404"> If the user not found</response>   
		/// <response code="500"> If internal error occured</response>
		[HttpGet(Name = "GetNotifications")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<CollectionWithPaginationData<NotificationDto>>> GetNotifications(string userId,
			[FromQuery] PaginationParams paginationParams, [FromQuery] string fromWhoId = null, [FromQuery] int notificationType = 0)
		{
			if (Guid.TryParse(userId, out Guid gUserId))
			{
				try
				{
					if (await _userService.CheckIfUserExists(gUserId))
					{
						PagedList<NotificationDto> notificationsToReturn = _notificationService.GetUserNotifications(gUserId, paginationParams, fromWhoId, notificationType);
						return Ok(this.CreateCollectionWithPagination(notificationsToReturn, paginationParams, "GetNotifications"));
					}
					else
					{
						return NotFound($"User: {userId} not found.");
					}
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error occured during getting the user notifications. User id: {id}", userId);
					return StatusCode(StatusCodes.Status500InternalServerError);
				}
			}
			else
			{
				return BadRequest($"{userId} is not valid guid.");
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
		public async Task<ActionResult<NotificationDto>> AddNotification(string userId, [FromBody] NotificationToAddDto notificationToAdd)
		{
			if (Guid.TryParse(userId, out Guid gUserId))
			{
				try
				{
					if (await _userService.CheckIfUserExists(gUserId))
					{
						var addedNotification = _notificationService.AddNotificationAsync(gUserId, notificationToAdd);

						return CreatedAtRoute("GetNotification",
							new
							{
								userId,
								notificationId = addedNotification.Id
							},
							notificationToAdd);
					}
					else
					{
						return NotFound($"User: {userId} not found.");
					}
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error occured during adding the notification. User id: {userId}", userId);
					return StatusCode(StatusCodes.Status500InternalServerError);
				}
			}
			else
			{
				return BadRequest($"{userId} is not valid guid.");
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
		public async Task<ActionResult> PartiallyUpdateNotification(string userId, string notificationId, JsonPatchDocument<NotificationToUpdateDto> patchDocument)
		{
			if (Guid.TryParse(userId, out Guid gUserId) && Guid.TryParse(notificationId, out Guid gNotificationId))
			{
				try
				{
					if (await _userService.CheckIfUserExists(gUserId))
					{
						if (await _notificationService.TryUpdateNotificationAsync(gNotificationId, patchDocument))
						{
							return NoContent();
						}
						else
						{
							return BadRequest();
						}
					}
					else
					{
						return NotFound($"User: {userId} not found.");
					}
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error occured during updating the notification. Notification id: {notificationId}", notificationId);
					return StatusCode(StatusCodes.Status500InternalServerError);
				}
			}
			else
			{
				return BadRequest($"User id: {userId} or notification id: {notificationId} is not valid guid.");
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
			if (Guid.TryParse(userId, out Guid gUserId) && Guid.TryParse(notificationId, out Guid gNotificationId))
			{
				try
				{
					if (await _userService.CheckIfUserExists(gUserId))
					{
						await _notificationService.DeleteNotificationAsync(gNotificationId);
						return NoContent();
					}
					else
					{
						return NotFound($"User: {userId} not found.");
					}
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error occured during removing the notification. Notification id: {notificationId}", notificationId);
					return StatusCode(StatusCodes.Status500InternalServerError);
				}
			}
			else
			{
				return BadRequest($"User id: {userId} or notification id: {notificationId} is not valid guid.");
			}
		}
	}
}
