using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyFaceApi.Api.IdentityServer.Application.DtoModels.User;
using MyFaceApi.IdentityServer.Application.Interfaces;
using SemesterProject.MyFaceApi.Helpers;
using System;
using System.Collections.Generic;

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
		//[HttpGet("users")]
		//public ActionResult<List<IdentityUserDto>> GetUsers()
		//{
		//	List<AppUser> usersToReturn = _identityServerDbContext.Users.ToList();
		//	try
		//	{

		//		return Ok(_mapper.Map<IEnumerable<IdentityUserDto>>(usersToReturn));

		//	}
		//	catch
		//	{
		//		_logger.LogError(ex, "Error occured during getting the user posts. User id: {user}", userId);
		//		return StatusCode(StatusCodes.Status500InternalServerError);
		//	}
		//}
	}
}
