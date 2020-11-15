using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyFaceApi.Entities;
using MyFaceApi.Models;
using MyFaceApi.Repository.Interfaceses;


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
		public async Task<ActionResult<BasicUserData>> GetUser(string userId)
		{
			try
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
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during getting the user. User id: {user}", userId);
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}


		/// <summary>
		/// Add user to database
		/// </summary>
		/// <param name="user"> of BasicUserData type</param>
		/// <returns>
		/// Returns httpcode 201 if the user has been created
		/// </returns>
		/// <response code="201"> Created if the user has been added</response>
		/// <response code="400"> If the user is not valid</response>    
		/// <response code="409"> If the user already exist</response>
		/// <response code="500"> If internal error occured</response>

		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status409Conflict)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<BasicUserData>> AddUser(BasicUserData user)
		{
			try
			{
				//ModelState.IsValid dosent work during testing
				if (user.Id == null || user.FirstName == null || user.LastName == null)
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
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during adding the user. User info: {user}", user);
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}

		/// <summary>
		/// Uptade user in database
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="patchDocument"></param>
		/// <returns>
		/// Returns status 204 no content if the user hase been updated
		/// </returns>
		/// <response code="204"> No content if the user hase been updated</response>
		/// <response code="400"> If the user is not valid</response>    
		/// <response code="404"> If the user not found</response>
		/// <response code="500"> If internal error occured</response>

		[HttpPatch("{userId}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult> PartiallyUpdateUser(string userId, JsonPatchDocument<BasicUserData> patchDocument)
		{
			try
			{
				if (Guid.TryParse(userId, out Guid gUserId))
				{
					User userFromRepo = await _userRepository.GetUserAsync(gUserId);
					if (userFromRepo == null)
					{
						return NotFound();
					}

					BasicUserData userToPatch = _mapper.Map<BasicUserData>(userFromRepo);

					patchDocument.ApplyTo(userToPatch, ModelState);

					if (!TryValidateModel(userToPatch))
					{
						return ValidationProblem(ModelState);
					}

					_mapper.Map(userToPatch, userFromRepo);

					await _userRepository.UpdateUserAsync(userFromRepo);
					return NoContent();
				}
				else
				{
					return BadRequest($"{userId} is not valid Guid.");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during updating the user. User id: {user}", userId);
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}
		/// <summary>
		/// Uptade user in database
		/// </summary>
		/// <param name="userId"></param>
		/// <returns>
		/// Returns status 204 no content if the user has been removed
		/// </returns>
		/// <response code="204"> No contentif the user has been removed</response>
		/// <response code="400"> If the user is not valid</response>    
		/// <response code="404"> If the user not found</response>
		/// <response code="500"> If internal error occured</response>

		[HttpDelete("{userId}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult> DeleteUser(string userId)
		{
			try
			{
				if (Guid.TryParse(userId, out Guid gUserId))
				{
					User userFromRepo = await _userRepository.GetUserAsync(gUserId);
					if (userFromRepo == null)
					{
						return NotFound();
					}
					await _userRepository.DeleteUserAsync(userFromRepo);
					return NoContent();
				}
				else
				{
					return BadRequest($"{userId} is not valid Guid.");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during removing the user. User id: {user}", userId);
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}
	}
}
