using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyFaceApi.Entities;
using MyFaceApi.Repository;
using MyFaceApi.Repository.Interfaceses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFaceApi.Controllers
{
	[Route("api/users/{userId}/posts")]
	[ApiController]
	public class PostsController : ControllerBase
	{
		private readonly ILogger<PostsController> _logger;
		private readonly IPostRepository _postRepository;
		private readonly IUserRepository _userRepository;
		private readonly IMapper _mapper;
		public PostsController(ILogger<PostsController> logger,
			IPostRepository postRepository,
			IUserRepository userRepository,
			IMapper mapper)
		{
			_logger = logger;
			_postRepository = postRepository;
			_userRepository = userRepository;
			_mapper = mapper;
			_logger.LogTrace("PostsController created");
		}

		/// <summary>
		/// Return the found user posts
		/// </summary>
		/// <param name="userId">User guid as a string </param>
		/// <returns>Found user</returns>
		/// <response code="200"> Returns the found user posts</response>
		/// <response code="400"> If parameter is not a valid guid</response>    
		/// <response code="404"> If user not found</response>   
		/// <response code="500"> If internal error occured</response>

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public ActionResult<List<Post>> GetPosts(string userId)
		{
			try
			{
				if (Guid.TryParse(userId, out Guid gUserId))
				{
					if (_userRepository.CheckIfUserExists(gUserId))
					{
						List<Post> userPosts = _postRepository.GetUserPosts(gUserId);
						return Ok(userPosts);
					}
					else
					{
						return NotFound($"User: {userId} not found.");
					}
				}
				else
				{
					return BadRequest($"{userId} is not valid Guid.");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during getting the user posts. User id: {user}", userId);
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}
	}
}
