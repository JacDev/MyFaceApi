using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyFaceApi.Api.Application.DtoModels.FriendsRelation;
using MyFaceApi.Api.Application.DtoModels.User;
using Pagination.Helpers;
using Pagination.DtoModels;
using Pagination.Extensions;
using MyFaceApi.Api.Application.Interfaces;
using System;
using System.Threading.Tasks;

namespace MyFaceApi.Api.Controllers
{
	[AllowAnonymous]
	[Route("api/users/{userId}/friends")]
	[ApiController]
	public class FriendsRelationsController : ControllerBase
	{

		private readonly IFriendsRelationService _relationService;
		private readonly ILogger<FriendsRelationsController> _logger;
		private readonly IUserService _userService;

		public FriendsRelationsController(IFriendsRelationService relationService,
			IUserService userService,
			ILogger<FriendsRelationsController> logger)
		{
			_logger = logger;
			_relationService = relationService;
			_userService = userService;
		}
		[HttpGet("{friendId}", Name = "GetRelation")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public ActionResult<FriendsRelationDto> GetRelation(string userId, string friendId)
		{
			if (Guid.TryParse(userId, out Guid gUserId) && Guid.TryParse(friendId, out Guid gFriendId))
			{
				try
				{
					FriendsRelationDto friendsRelation = _relationService.GetFriendRelation(gUserId, gFriendId);
					return Ok(friendsRelation);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error occured during getting the friends relation. Users id: {userId} and {friendId}", userId, friendId);
					return StatusCode(StatusCodes.Status500InternalServerError);
				}
			}
			else
			{
				return BadRequest($"{userId} or {friendId} is not valid guid.");
			}
		}
		[HttpGet(Name = "GetFriends")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<CollectionWithPaginationData<UserDto>>> GetUserFriends(string userId, [FromQuery] PaginationParams paginationParams)
		{
			if (Guid.TryParse(userId, out Guid gUserId))
			{
				try
				{
					paginationParams.Skip = (paginationParams.PageNumber - 1) * paginationParams.PageSize;
					if (await _userService.CheckIfUserExists(gUserId))
					{
						PagedList<UserDto> friendsToReturn = await _relationService.GetUserFriends(gUserId, paginationParams);
						return Ok(this.CreateCollectionWithPagination(friendsToReturn, paginationParams, "GetFriends"));
					}
					else
					{
						return NotFound($"User: {userId} not found.");
					}
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error occured during getting the user relations. User id: {user}", userId);
					return StatusCode(StatusCodes.Status500InternalServerError);
				}
			}
			else
			{
				return BadRequest($"{userId} is not valid guid.");
			}
		}
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<FriendsRelationDto>> AddRelation(string userId, FriendsRelationToAddDto relationToAdd)
		{
			try
			{
				if (Guid.TryParse(userId, out Guid gUserId))
				{
					if (await _userService.CheckIfUserExists(gUserId) && await _userService.CheckIfUserExists(relationToAdd.FriendId))
					{
						FriendsRelationDto addedRealtion = await _relationService.AddRelationAsync(gUserId, relationToAdd);
						return CreatedAtRoute("GetRelation",
							new { userId, friendId = addedRealtion.FriendId },
							addedRealtion);
					}
					else
					{
						return NotFound($"User: {userId} or friend: {relationToAdd.FriendId} not found.");
					}
				}
				else
				{
					return BadRequest($"{userId} is not valid guid.");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during adding the user relation. User id: {user}, friend id {FriendId}", userId, relationToAdd.FriendId);
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}
		[HttpDelete("{friendId}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult> DeleteRelation(string userId, string friendId)
		{
			try
			{
				if (Guid.TryParse(userId, out Guid gUserId) && Guid.TryParse(friendId, out Guid gFriendId))
				{
					if (await _userService.CheckIfUserExists(gUserId) && await _userService.CheckIfUserExists(gFriendId))
					{
						await _relationService.DeleteRelationAsync(gUserId, gFriendId);
						return NoContent();
					}
					else
					{
						return NotFound($"User: {userId} or friend: {gFriendId} not found.");
					}
				}
				else
				{
					return BadRequest($"{userId} or {friendId} is not valid guid.");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during removing the friends relation. Users id: {userId} and {friendId}", userId, friendId);
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}
	}
}
