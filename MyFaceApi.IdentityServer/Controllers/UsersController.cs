using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyFaceApi.Api.DataAccess.ModelsBasicInfo;
using MyFaceApi.IdentityServer.DataAccess.Data;
using MyFaceApi.IdentityServer.DataAccess.Entities;
using SemesterProject.MyFaceApi.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFaceApi.IdentityServer.Controllers
{
	public class UsersController : Controller
	{
		private readonly IdentityServerDbContext _identityServerDbContext;
		private readonly IMapper _mapper;
		private readonly UserManager<AppUser> _userManager;

		public UsersController(IdentityServerDbContext identityServerDbContext,
			IMapper mapper, UserManager<AppUser> userManager)
		{
			_identityServerDbContext = identityServerDbContext;
			_mapper = mapper;
			_userManager = userManager;
		}

		[HttpGet("users/{userId}")]
		public ActionResult<BasicUserData> GetUser(string userId)
		{
			if(Guid.TryParse(userId, out Guid gUserId))
			{
				AppUser user = _identityServerDbContext.Users.FirstOrDefault(x => x.Id == gUserId);
				return _mapper.Map<BasicUserData>(user);
			}
			else
			{
				return BadRequest();
			}
		}
		[HttpGet("users/getall")]
		public ActionResult<List<BasicUserData>> GetUsersById([ModelBinder(typeof(ArrayModelBinder))] string[] ids)
		{
			List<AppUser> usersToReturn = new List<AppUser>();
			try
			{
				foreach(var id in ids)
				{
					if(Guid.TryParse(id, out Guid gId))
					{
						usersToReturn.Add(_identityServerDbContext.Users.FirstOrDefault(x => x.Id == gId));
					}
					else
					{
						return BadRequest($"{gId} is not valid guid.");
					}
				}
				return Ok(_mapper.Map<IEnumerable<BasicUserData>>(usersToReturn));
			
			}
			catch (Exception ex)
			{
				//_logger.LogError(ex, "Error occured during getting the user posts. User id: {user}", userId);
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}
		[HttpGet]
		public async Task<IActionResult> Pupulate()
		{
			var fixture = new Fixture().Customize(new AutoMoqCustomization());
			List<AppUser> appUsers = new List<AppUser>();
			fixture.AddManyTo(appUsers, 10);
			foreach(var user in appUsers)
			{
				await _userManager.CreateAsync(user);
			}
			
			await _identityServerDbContext.SaveChangesAsync();
			return Ok();
		}
		[HttpGet("users")]
		public ActionResult<List<BasicUserData>> GetUsers()
		{
			List<AppUser> usersToReturn = _identityServerDbContext.Users.ToList();
			try
			{
				
				return Ok(_mapper.Map<IEnumerable<BasicUserData>>(usersToReturn));

			}
			catch (Exception ex)
			{
				//_logger.LogError(ex, "Error occured during getting the user posts. User id: {user}", userId);
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}
	}
}
