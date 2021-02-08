using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyFaceApi.Api.Application.DtoModels.Post;
using MyFaceApi.Api.Application.Interfaces;
using Pagination.Helpers;
using Pagination.DtoModels;
using Pagination.Extensions;
using System;
using System.Threading.Tasks;

namespace MyFaceApi.Api.Controllers
{
	[Route("api/users/{userId}/posts")]
	[ApiController]
	public class PostsController : ControllerBase
	{
		private readonly ILogger<PostsController> _logger;
		private readonly IPostService _postService;
		private readonly IUserService _userService;

		public PostsController(ILogger<PostsController> logger,
			IPostService postService,
			IUserService userService)
		{
			_logger = logger;
			_postService = postService;
			_userService = userService;
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
		public ActionResult<PostDto> GetPost(string postId)
		{
			if (Guid.TryParse(postId, out Guid gPostId))
			{
				try
				{
					return Ok(_postService.GetPost(gPostId));
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error occured during getting the post. Post id: {postId}", postId);
					return StatusCode(StatusCodes.Status500InternalServerError);
				}
			}
			else
			{
				return BadRequest($"{postId} is not valid guid.");
			}
		}
		/// <summary>
		/// Return the found user posts
		/// </summary>
		/// <param name="userId">User guid as a string </param>
		/// <param name="paginationParams"></param>
		/// <returns>Found user posts</returns>
		/// <response code="200"> Returns the found user posts</response>
		/// <response code="400"> If parameter is not a valid guid</response>    
		/// <response code="404"> If user not found</response>   
		/// <response code="500"> If internal error occured</response>
		[HttpGet(Name = "GetPosts")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<CollectionWithPaginationData<PostDto>>> GetPosts(string userId, [FromQuery] PaginationParams paginationParams)
		{
			if (Guid.TryParse(userId, out Guid gUserId))
			{
				try
				{
					if (await _userService.CheckIfUserExists(gUserId))
					{
						PagedList<PostDto> postsToReturn = _postService.GetUserPosts(gUserId, paginationParams);
						return Ok(this.CreateCollectionWithPagination(postsToReturn, paginationParams, "GetPosts"));
					}
					else
					{
						return NotFound($"User: {userId} not found.");
					}
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error occured during getting the user posts. User id: {user}", userId);
					return StatusCode(StatusCodes.Status500InternalServerError);
				}
			}
			else
			{
				return BadRequest($"{userId} is not valid guid.");
			}
		}
		[HttpGet("latest", Name = "GetLatestPosts")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<CollectionWithPaginationData<PostDto>>> GetLatestPosts(string userId, [FromQuery] PaginationParams paginationParams)
		{
			if (Guid.TryParse(userId, out Guid gUserId))
			{
				try
				{
					if (await _userService.CheckIfUserExists(gUserId))
					{
						PagedList<PostDto> postsToReturn = _postService.GetLatestFriendsPosts(gUserId, paginationParams);
						return Ok(this.CreateCollectionWithPagination(postsToReturn, paginationParams, "GetLatestPosts"));
					}
					else
					{
						return NotFound($"User: {userId} not found.");
					}
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error occured during getting the user posts. User id: {user}", userId);
					return StatusCode(StatusCodes.Status500InternalServerError);
				}
			}
			else
			{
				return BadRequest($"{userId} is not valid guid.");
			}
		}
		/// <summary>
		/// Add post to database
		/// </summary>
		/// <param name="userId">User guid as a string </param>
		/// <param name="postToAdd"></param>
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
		public async Task<ActionResult<PostDto>> AddPost(string userId, [FromForm] PostToAddDto postToAdd)
		{
			if (Guid.TryParse(userId, out Guid gUserId))
			{
				try
				{
					if (await _userService.CheckIfUserExists(gUserId))
					{
						PostDto addedPost = await _postService.AddPostAsync(gUserId, postToAdd);
						return CreatedAtRoute("GetPost",
							new { userId, postId = addedPost.Id },
							addedPost);
					}
					else
					{
						return NotFound($"User: {userId} not found.");
					}
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error occured during adding the user post. User id: {user}", userId);
					return StatusCode(StatusCodes.Status500InternalServerError);
				}
			}
			else
			{
				return BadRequest($"{userId} is not valid guid.");
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
		/// <response code="500"> If internal error occured</response>
		[HttpPatch("{postId}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult> PartiallyUpdatePost(string postId, JsonPatchDocument<PostToUpdateDto> patchDocument)
		{
			if (Guid.TryParse(postId, out Guid gUserId))
			{
				try
				{
					if (await _postService.TryUpdatePostAsync(gUserId, patchDocument))
					{
						return NoContent();
					}
					else
					{
						return BadRequest();
					}
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error occured during updating the post. Post id: {postId}", postId);
					return StatusCode(StatusCodes.Status500InternalServerError);
				}
			}
			else
			{
				return BadRequest($"{postId} is not valid guid.");
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
		/// <response code="500"> If internal error occured</response>
		[HttpDelete("{postId}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult> DeletePost(string postId)
		{
			if (Guid.TryParse(postId, out Guid gPostId))
			{
				try
				{
					await _postService.DeletePostAsync(gPostId);
					return NoContent();
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error occured during removing the post. Post id: {postId}", postId);
					return StatusCode(StatusCodes.Status500InternalServerError);
				}
			}
			else
			{
				return BadRequest($"{postId} is not valid guid.");
			}
		}
		[HttpGet("image/{image}")]
		public IActionResult Image(string image)
		{
			try
			{
				return _postService.StreamImage(image);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during streaming image: {image}", image);
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}
		[HttpPost("{postId}")]
		public async Task<ActionResult<string>> SetProfilePicture(string userId, string postId)
		{
			if (Guid.TryParse(postId, out Guid gPostiId) && Guid.TryParse(userId, out Guid gUserId))
			{
				try
				{
					if (await _userService.CheckIfUserExists(gUserId))
					{
						return Ok(await _postService.SetProfilePicture(gUserId, gPostiId));
					}
					else
					{
						return NotFound($"User: {userId} not found.");
					}
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error occured during adding the user post. User id: {user}", userId);
					return StatusCode(StatusCodes.Status500InternalServerError);
				}
			}
			else
			{
				return BadRequest($"{userId} is not valid guid.");
			}
		}
	}
}