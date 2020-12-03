using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyFaceApi.Api.DataAccess.Entities;
using MyFaceApi.Api.DataAccess.ModelsBasicInfo;
using MyFaceApi.Api.IdentityServerAccess;
using MyFaceApi.Api.Models.UserModels;
using MyFaceApi.Api.Repository.Interfaces;


namespace MyFaceApi.Api.Controllers
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
					var userFromRepo = await _userRepository.GetUserAsync(gUserId);
					//return Ok(_mapper.Map<BasicUserData>(userFromRepo));
					//Response.Headers.Add("Access-Control-Allow-Origin", "*");
					return Ok(userFromRepo);
				}
				else
				{
					return BadRequest($"{userId} is not valid guid.");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during getting the user. User id: {user}", userId);
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public ActionResult GetUser()
		{
				var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
				return RedirectToAction("GetUser", new { userId });
		}
		/// <summary>
		/// Updade user in the database
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
					var userFromRepo = await _userRepository.GetUserAsync(gUserId);
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

					//await _userRepository.UpdateUserAsync(userFromRepo);
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
	}
}
