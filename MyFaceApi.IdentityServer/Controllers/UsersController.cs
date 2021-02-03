using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyFaceApi.Api.IdentityServer.Application.DtoModels.User;
using MyFaceApi.IdentityServer.Application.Helpers;
using MyFaceApi.IdentityServer.Application.Interfaces;
using Pagination.Extensions;
using Pagination.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyFaceApi.IdentityServer.Controllers
{
	public class UsersController : Controller
	{
		private readonly IIdentityUserService _identityUserService;

		public UsersController(IIdentityUserService identityUserService)
		{
			_identityUserService = identityUserService;
		}

		[HttpGet("users/{userId}")]
		public ActionResult<IdentityUserDto> GetUser(string userId)
		{
			if (Guid.TryParse(userId, out Guid gUserId))
			{
				return _identityUserService.GetUser(gUserId);
			}
			else
			{
				return BadRequest();
			}
		}
		[HttpGet("users/getall")]
		public ActionResult<List<IdentityUserDto>> GetUsersById([ModelBinder(typeof(ArrayModelBinder))] string[] ids)
		{
			try
			{
				List<IdentityUserDto> usersToReturn = _identityUserService.GetUsers(ids);
				return Ok(usersToReturn);
			}
			catch
			{
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}
		[HttpGet("users/with", Name ="GetUsersWith")]
		public ActionResult<PagedList<IdentityUserDto>> GetUsersWith(PaginationParams paginationParams, [FromQuery] string searchString = null)
		{
			try
			{				
				PagedList<IdentityUserDto> usersToReturn = _identityUserService.GetUsers(searchString, paginationParams);
				if (searchString != null)
				{
					var queryParams = new Dictionary<object, object>
				{
					{ nameof(searchString), searchString }
				};
					return Ok(this.CreateCollectionWithPagination(usersToReturn, paginationParams, "GetUsersWith", queryParams));
				}
				return Ok(this.CreateCollectionWithPagination(usersToReturn, paginationParams, "GetUsersWith"));
			}
			catch
			{
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}
		[HttpPost("users/{userId}")]
		public async Task<ActionResult<IdentityUserDto>> AddProfilePath(string userId, [FromBody] string path)
		{
			if (Guid.TryParse(userId, out Guid gUserId) && !string.IsNullOrEmpty(path))
			{
				return await _identityUserService.AddProfileImage(gUserId, path);
			}
			else
			{
				return BadRequest();
			}
		}
	}
}
