using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyFaceApi.DataAccess.Entities;
using MyFaceApi.Models.PostModels;
using MyFaceApi.Repository.Interfaces;
using System;
using System.Collections.Generic;
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
		/// Return the found post
		/// </summary>
		/// <param name="postId">Post guid as a string </param>
		/// <returns>Found post</returns>
		/// <response code="200"> Returns the found post</response>
		/// <response code="400"> If parameter is not a valid guid</response>    
		/// <response code="404"> If post not found</response>   
		/// <response code="500"> If internal error occured</response>
		[HttpGet("{postId}", Name = "GetPost")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public ActionResult<Post> GetPost(string postId)
		{
			try
			{
				if (Guid.TryParse(postId, out Guid gPostId))
				{
					Post postToReturn = _postRepository.GetPost(gPostId);
					if (postToReturn != null)
					{
						return Ok(postToReturn);
					}					
					else
					{
						return NotFound();
					}
				}
				else
				{
					return BadRequest($"{postId} is not valid guid.");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during getting the post. Post id: {postId}", postId);
				return StatusCode(StatusCodes.Status500InternalServerError);
			}		
		}
		/// <summary>
		/// Return the found user posts
		/// </summary>
		/// <param name="userId">User guid as a string </param>
		/// <returns>Found user posts</returns>
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
					return BadRequest($"{userId} is not valid guid.");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during getting the user posts. User id: {user}", userId);
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}
		/// <summary>
		/// Add post to database
		/// </summary>
		/// <param name="userId">User guid as a string </param>
		/// <param name="post"></param>
		/// <returns>Added post</returns>
		/// <response code="201"> Return created post</response>
		/// <response code="400"> If parameter is not a valid guid</response>    
		/// <response code="404"> If user not found</response>   
		/// <response code="500"> If internal error occured</response>
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<Post>> AddPost(string userId, PostToAdd post)
		{
			try
			{
				if (Guid.TryParse(userId, out Guid gUserId))
				{
					if (_userRepository.CheckIfUserExists(gUserId))
					{
						Post postEntity = _mapper.Map<Post>(post);
						postEntity.UserId = gUserId;
						postEntity = await _postRepository.AddPostAsync(postEntity);

						return CreatedAtRoute("GetPost",
							new { userId, postId = postEntity.Id },
							postEntity);
					}
					else
					{
						return NotFound($"User: {userId} not found.");
					}
				}
				else
				{
					return BadRequest($"{userId} is not valid guid.");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during adding the user post. User id: {user}", userId);
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}
		/// <summary>
		/// Update post in the database
		/// </summary>
		/// <param name="postId"></param>
		/// <param name="patchDocument"></param>
		/// <returns>
		/// Status 204 no content if the post has been updated
		/// </returns>
		/// <response code="204"> No content if the post has been updated</response>
		/// <response code="400"> If the postId is not valid guid</response>    
		/// <response code="404"> If the post not found</response>
		/// <response code="500"> If internal error occured</response>
		[HttpPatch("{postId}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult> PartiallyUpdatePost(string postId, JsonPatchDocument<PostToUpdate> patchDocument)
		{
			try
			{
				if (Guid.TryParse(postId, out Guid gUserId))
				{
					Post postFromRepo = _postRepository.GetPost(gUserId);
					if (postFromRepo == null)
					{
						return NotFound();
					}
					PostToUpdate postToPatch = _mapper.Map<PostToUpdate>(postFromRepo);
					patchDocument.ApplyTo(postToPatch, ModelState);

					if (!TryValidateModel(postToPatch))
					{
						return ValidationProblem(ModelState);
					}

					_mapper.Map(postToPatch, postFromRepo);

					await _postRepository.UpdatePostAsync(postFromRepo);
					return NoContent();
				}
				else
				{
					return BadRequest($"{postId} is not valid guid.");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during updating the post. Post id: {postId}", postId);
				return StatusCode(StatusCodes.Status500InternalServerError);

			}
		}
		/// <summary>
		/// Remove the post from the database
		/// </summary>
		/// <param name="postId"></param>
		/// <returns>
		/// Status 204 no content if the post has been removed
		/// </returns>
		/// <response code="204"> No content if the post has been removed</response>
		/// <response code="400"> If the post is not valid</response>    
		/// <response code="404"> If the post not found</response>
		/// <response code="500"> If internal error occured</response>
		[HttpDelete("{postId}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult> DeletePost(string postId)
		{
			try
			{
				if (Guid.TryParse(postId, out Guid gPostId))
				{
					Post postFromRepo = _postRepository.GetPost(gPostId);
					if (postFromRepo == null)
					{
						return NotFound();
					}
					await _postRepository.DeletePostAsync(postFromRepo);
					return NoContent();
				}
				else
				{
					return BadRequest($"{postId} is not valid guid.");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during removing the post. Post id: {postId}", postId);
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}
	}
}
