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
		public async Task<ActionResult<UserToReturn>> GetUser(string userId)
		{
			if (Guid.TryParse(userId, out Guid gUserId))
			{
				User userFromRepo = await _userRepository.GetUserAsync(gUserId);
				if (userFromRepo == null)
				{
					return NotFound();
				}
				return Ok(_mapper.Map<UserToReturn>(userFromRepo));
			}
			else
			{
				return BadRequest($"{userId} is not valid Guid.");
			}
		}


		// POST api/<ValuesController>
		[HttpPost]
		public void Post([FromBody] string value)
		{
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
