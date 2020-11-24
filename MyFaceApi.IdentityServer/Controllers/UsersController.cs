using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyFaceApi.Api.DataAccess.ModelsBasicInfo;
using MyFaceApi.IdentityServer.DataAccess.Data;
using MyFaceApi.IdentityServer.DataAccess.Entities;
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
				DataAccess.Entities.AppUser user = _identityServerDbContext.Users.FirstOrDefault(x => x.Id == gUserId);
				return _mapper.Map<BasicUserData>(user);
			}
			else
			{
				return BadRequest();
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
	}
}
