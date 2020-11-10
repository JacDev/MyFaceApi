using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyFaceApi.Entities;
using MyFaceApi.Models;
using MyFaceApi.Repository;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyFaceApi.Controllers
{
	[Produces("application/json")]
	[Route("api/[controller]")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		private readonly ILogger<UsersController> _logger;
		private readonly IUserRepository _userRepository;
		private readonly IMapper _mapper;
		public UsersController(ILogger<UsersController> logger,
			IUserRepository userRepository,
			IMapper mapper)
		{
			_logger = logger;
			_userRepository = userRepository;
			_mapper = mapper;
			_logger.LogInformation("Example controller created");
		}
		/// <summary>
		/// Return the found user
		/// </summary>
		/// <param name="userId">User guid as a string </param>
		/// <returns>Found user</returns>
		/// <response code="200">Returns the found user</response>
		/// <response code="400">If parameter is not a valid guid</response>    
		/// <response code="404">If user not found</response>   
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[HttpGet("{userId}", Name = "GetUser")]
		public async Task<ActionResult<BasicUserData>> GetUser(string userId)
		{
			if (Guid.TryParse(userId, out Guid gUserId))
			{
				User userFromRepo = await _userRepository.GetUserAsync(gUserId);
				if (userFromRepo == null)
				{
					return NotFound();
				}
				return Ok(_mapper.Map<BasicUserData>(userFromRepo));
			}
			else
			{
				return BadRequest($"{userId} is not valid Guid.");
			}
		}


		/// <summary>
		/// Add user to database
		/// </summary>
		/// <param name="user"></param>
		/// <returns>
		/// Returns added user, HttpCode 400 or HttpCode 409
		/// </returns>
		/// <response code="200">Returns added user</response>
		/// <response code="400"> If user is not valid</response>    
		/// <response code="409"> If user already exist</response>


		[HttpPost]
		public async Task<ActionResult<BasicUserData>> AddUser(BasicUserData user)
		{
			//ModelState.IsValid dosent work during testing
			if (user.Id == null || user.FirstName==null || user.LastName == null)
			{
				return BadRequest("One or more validation errors occurred.");
			}
			User userEntity = _mapper.Map<User>(user);
			if (!_userRepository.CheckIfUserExists(user.Id))
			{
				await _userRepository.AddUserAcync(userEntity);
			}
			else
			{
				return Conflict($"{user.Id} already exist.");
			}

			BasicUserData userToReturn = _mapper.Map<BasicUserData>(userEntity);

			return CreatedAtRoute("GetUser",
			new { userId = userToReturn.Id },
			userToReturn);
		}

		// PUT api/<ValuesController>/5
		[HttpPut("{id}")]
		public void Put(int id, [FromBody] string value)
		{
		}

		// DELETE api/<ValuesController>/5
		[HttpDelete("{id}")]
		public void Delete(int id)
		{
		}
	}
}
