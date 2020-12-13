using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyFaceApi.Api.Application.DtoModels.PostReaction;
using MyFaceApi.Api.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyFaceApi.Api.Controllers
{
	[AllowAnonymous]
	[Route("api/users/{userId}/posts/{postId}/reactions")]
	[ApiController]
	public class PostReactionsController : ControllerBase
	{
		private readonly ILogger<PostReactionsController> _logger;
		private readonly IPostService _postService;
		private readonly IUserService _userService;
		private readonly IPostReactionService _postReactionService;
		public PostReactionsController(ILogger<PostReactionsController> logger,
			IPostService postService,
			IUserService userService,
			IPostReactionService postReactionService)
		{
			_logger = logger;
			_postService = postService;
			_userService = userService;
			_postReactionService = postReactionService;
			_logger.LogTrace("PostReactionController created");
		}
		/// <summary>
		/// Add comment to database
		/// </summary>
		/// <param name="userId">User guid as a string </param>
		/// <param name="postId">Post guid as a string</param>
		/// <param name="reactionToAdd">Reaction to add of type ReactionToAdd</param>
		/// <returns>Added reaction</returns>
		/// <response code="201"> Return created reaction</response>
		/// <response code="400"> If parameter is not a valid guid</response>    
		/// <response code="404"> If user or post not found</response>   
		/// <response code="500"> If internal error occured</response>
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<PostReactionDto>> AddPostReaction(string userId, string postId, [FromBody] PostReactionToAddDto reactionToAdd)
		{
			if (Guid.TryParse(postId, out Guid gPostId) && Guid.TryParse(userId, out Guid gUserId))
			{
				try
				{
					if (_postService.CheckIfPostExists(gPostId) && await _userService.CheckIfUserExists(gUserId))
					{

						PostReactionDto addedReaction = await _postReactionService.AddPostReactionAsync(gPostId, reactionToAdd);

						return CreatedAtRoute("GetReaction",
							new
							{
								userId,
								postId = addedReaction.PostId,
								reactionId = addedReaction.Id
							},
							addedReaction);
					}
					else
					{
						return NotFound($"User: {userId} or post {postId} not found.");
					}
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error occured during adding the reaction. Post id: {postId}", postId);
					return StatusCode(StatusCodes.Status500InternalServerError);
				}
			}
			else
			{
				return BadRequest($"{userId} or {postId} is not valid guid.");
			}
		}
		/// <summary>
		/// Return the found post reaction
		/// </summary>
		/// <param name="postId">Post guid as a string </param>
		/// <returns>Found post reactions</returns>
		/// <response code="200"> Returns the found post reactions</response>
		/// <response code="400"> If parameter is not a valid guid</response>    
		/// <response code="404"> If post not found</response>   
		/// <response code="500"> If internal error occured</response>
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public ActionResult<List<PostReactionDto>> GetPostReactions(string postId)
		{
			if (Guid.TryParse(postId, out Guid gPostId))
			{
				try
				{
					if (_postService.CheckIfPostExists(gPostId))
					{
						List<PostReactionDto> reactionsToReturn = _postReactionService.GetPostReactions(gPostId);
						return Ok(reactionsToReturn);
					}
					else
					{
						return NotFound($"Post: {postId} doesnt exists.");
					}
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error occured during getting reactions of the post: {postId}", postId);
					return StatusCode(StatusCodes.Status500InternalServerError);
				}
			}
			else
			{
				return BadRequest($"{postId} is not valid Guid.");
			}
		}
		/// <summary>
		/// Return the found reaction
		/// </summary>
		/// <param name="postId">Post guid as a string </param>
		/// <param name="reactionId">Reaction guid as a string </param>
		/// <returns>Found reaction</returns>
		/// <response code="200"> Returns the found reaction</response>
		/// <response code="400"> If parameter is not a valid guid</response>    
		/// <response code="404"> If post or comment not found</response>   
		/// <response code="500"> If internal error occured</response>
		[HttpGet("{reactionId}", Name = "GetReaction")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public ActionResult<List<PostReactionDto>> GetPostReaction(string postId, string reactionId)
		{
			if (Guid.TryParse(postId, out Guid gPostId) && Guid.TryParse(reactionId, out Guid gReactionId))
			{
				try
				{
					if (_postService.CheckIfPostExists(gPostId))
					{
						PostReactionDto postReaction = _postReactionService.GetPostReaction(gReactionId);
						return Ok(postReaction);
					}
					else
					{
						return NotFound($"Post: {postId} not found.");
					}
				}

				catch (Exception ex)
				{
					_logger.LogError(ex, "Error occured during getting the reaction: postId: {postId}, reactionId: {reactionId}", postId, reactionId);
					return StatusCode(StatusCodes.Status500InternalServerError);
				}
			}
			else
			{
				return BadRequest($"{postId} or {reactionId} is not valid guid.");
			}
		}
		/// <summary>
		/// Update reaction in database
		/// </summary>
		/// <param name="reactionId"></param>
		/// <param name="patchDocument"></param>
		/// <returns>
		/// Status 204 no content if the reaction has been updated
		/// </returns>
		/// <response code="204"> No content if the reaction has been updated</response>
		/// <response code="400"> If the reaction is not valid</response>    
		/// <response code="404"> If the reaction not found</response>
		/// <response code="500"> If internal error occured</response>
		[HttpPatch("{reactionId}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult> PartiallyUpdatePostReaction(string reactionId, JsonPatchDocument<PostReactionToUpdate> patchDocument)
		{
			if (Guid.TryParse(reactionId, out Guid gReactionId))
			{
				try
				{
					if (await _postReactionService.TryUpdatePostReactionAsync(gReactionId, patchDocument))
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
					_logger.LogError(ex, "Error occured during updating the reaction. Reaction id: {reactionId}", reactionId);
					return StatusCode(StatusCodes.Status500InternalServerError);

				}
			}
			else
			{
				return BadRequest($"{reactionId} is not valid guid.");
			}
		}
		/// <summary>
		/// Remove a reaction from the database
		/// </summary>
		/// <param name="postId"></param>
		/// <param name="fromWho"></param>
		/// <returns>
		/// Status 204 no content if the reaction has been removed
		/// </returns>
		/// <response code="204"> No content if the reaction has been removed</response>
		/// <response code="400"> If the post is not valid</response>    
		/// <response code="404"> If the post not found</response>
		/// <response code="500"> If internal error occured</response>
		[HttpDelete("{fromWho}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult> DeletePostReaction(string postId, string fromWho)
		{

			if (Guid.TryParse(postId, out Guid gPostId) && Guid.TryParse(fromWho, out Guid gFromWho))
			{
				try
				{
					if (_postService.CheckIfPostExists(gPostId) && await _userService.CheckIfUserExists(gFromWho))
					{
						await _postReactionService.DeletePostReactionAsync(gFromWho, gPostId);
						return NoContent();
					}
					else
					{
						return NotFound($"User: {gFromWho} or post {postId} not found.");
					}
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error occured during removing the reaction. FromWho id: {fromWho}, post id: {postId}", fromWho, postId);
					return StatusCode(StatusCodes.Status500InternalServerError);
				}
			}
			else
			{
				return BadRequest($"{fromWho} or {postId} is not valid Guid.");
			}
		}
	}
}
