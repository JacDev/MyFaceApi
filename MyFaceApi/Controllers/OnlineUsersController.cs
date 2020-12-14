using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyFaceApi.Api.Application.DtoModels.User;
using MyFaceApi.Api.Application.Interfaces;
using Pagination.Helpers;
using Pagination.DtoModels;
using Pagination.Extensions;
using System;
using System.Threading.Tasks;

namespace MyFaceApi.Api.Controllers
{
	[AllowAnonymous]
	[Route("api/users/{userId}/onlinefriends")]
	[ApiController]
	public class OnlineUsersController : ControllerBase
	{
		private readonly IOnlineUsersService _onlineUsersService;
		private readonly ILogger<OnlineUsersController> _logger;
		private readonly IUserService _userService;
		public OnlineUsersController(IOnlineUsersService onlineUsersService,
			IFriendsRelationService friendsRelationService,
			ILogger<OnlineUsersController> logger,
			IUserService userService)
		{
			_onlineUsersService = onlineUsersService;
			_logger = logger;
			_userService = userService;
		}
		[HttpGet(Name = "GetOnlineFriends")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<CollectionWithPaginationData<UserDto>>> GetOnlineFriends(string userId, [FromQuery] PaginationParams paginationParams)
		{

			if (Guid.TryParse(userId, out Guid gUserId))
			{
				try
				{
					paginationParams.Skip = (paginationParams.PageNumber - 1) * paginationParams.PageSize;
					if (await _userService.CheckIfUserExists(gUserId))
					{
						PagedList<UserDto> usersToReturn = await _onlineUsersService.GetOnlineFriends(gUserId, paginationParams);
						return Ok(this.CreateCollectionWithPagination(usersToReturn, paginationParams, "GetOnlineFriends"));
					}
					else
					{
						return NotFound($"User: {userId} not found.");
					}
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error occured during getting the online friends. User id: {user}", userId);
					return StatusCode(StatusCodes.Status500InternalServerError);
				}
			}
			else
			{
				return BadRequest($"{userId} is not valid guid.");
			}
		}

	}
}
