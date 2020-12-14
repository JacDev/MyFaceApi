using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyFaceApi.Api.Application.DtoModels.User;
using MyFaceApi.Api.Application.Interfaces;
using Pagination.Helpers;
using Pagination.DtoModels;
using Pagination.Extensions;
using System.Collections.Generic;

namespace MyFaceApi.Api.Controllers
{
	[AllowAnonymous]
	[Produces("application/json")]
	[Route("api/[controller]")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		private readonly ILogger<UsersController> _logger;
		private readonly IUserService _userService;

		public UsersController(ILogger<UsersController> logger,
			IUserService userService)
		{
			_logger = logger;
			_userService = userService;
			_logger.LogTrace("UserController created");
		}

		/// <summary>
		/// Return the found user
		/// </summary>
		/// <param name="userId">User guid as a string </param>
		/// <returns>Found user</returns>
		/// <response code="200"> Returns the found user</response>
		/// <response code="400"> If parameter is not a valid guid</response>    
		/// <response code="404"> If user not found</response>   
		/// <response code="500"> If internal error occured</response>
		[HttpGet("{userId}", Name = "GetUser")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<UserDto>> GetUser(string userId)
		{
			if (Guid.TryParse(userId, out Guid gUserId))
			{
				try
				{
					UserDto userFromRepo = await _userService.GetUserAsync(gUserId);
					return Ok(userFromRepo);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error occured during getting the user. User id: {user}", userId);
					return StatusCode(StatusCodes.Status500InternalServerError);
				}
			}
			else
			{
				return BadRequest($"{userId} is not valid guid.");
			}
		}
		[HttpGet("find", Name = "FindUsers")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<UserDto>> GetFoundUsers([FromQuery] PaginationParams paginationParams, [FromQuery] string searchName = null)
		{
			try
			{
				PagedList<UserDto> foundUsers = await _userService.GetUsersAsync(searchName, paginationParams);
				if (searchName != null)
				{
					var queryParams = new Dictionary<object, object>
				{
					{ nameof(searchName), searchName }
				};
					return Ok(this.CreateCollectionWithPagination(foundUsers, paginationParams, "FindUsers", queryParams));
				}
				return Ok(this.CreateCollectionWithPagination(foundUsers, paginationParams, "FindUsers"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during getting users with: {searchString}", searchName);
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}
	}
}