using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyFaceApi.Api.DataAccess.Entities;
using MyFaceApi.Api.Models.PostReactionModels;
using MyFaceApi.Api.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyFaceApi.Api.Controllers
{
	[Route("api/users/{userId}/posts/{postId}/reactions")]
	[ApiController]
	public class PostReactionsController : ControllerBase
	{
		private readonly ILogger<PostReactionsController> _logger;
		private readonly IPostRepository _postRepository;
		private readonly IUserRepository _userRepository;
		private readonly IPostReactionRepository _postReactionRepository;
		private readonly IMapper _mapper;
		public PostReactionsController(ILogger<PostReactionsController> logger,
			IPostRepository postRepository,
			IUserRepository userRepository,
			IMapper mapper,
			IPostReactionRepository postReactionRepository)
		{
			_logger = logger;
			_postRepository = postRepository;
			_userRepository = userRepository;
			_mapper = mapper;
			_postReactionRepository = postReactionRepository;
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
		public async Task<ActionResult<PostReactionToAdd>> AddPostReaction(string userId, string postId, [FromBody] PostReactionToAdd reactionToAdd)
		{
			try
			{
				if (Guid.TryParse(postId, out Guid gPostId) && Guid.TryParse(userId, out Guid gUserId))
				{
					if (_postRepository.CheckIfPostExists(gPostId) && await _userRepository.CheckIfUserExists(gUserId))
					{
						PostReaction postReactionEntity = _mapper.Map<PostReaction>(reactionToAdd);
						postReactionEntity.WhenAdded = DateTime.Now;
						postReactionEntity.PostId = gPostId;
						postReactionEntity = await _postReactionRepository.AddPostReactionAsync(postReactionEntity);

						return CreatedAtRoute("GetReaction",
							new
							{
								userId,
								postId = postReactionEntity.PostId,
								reactionId = postReactionEntity.Id
							},
							postReactionEntity);
					}
					else
					{
						return NotFound($"User: {userId} or post {postId} not found.");
					}
				}
				else
				{
					return BadRequest($"{userId} or {postId} is not valid guid.");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during adding the reaction. Post id: {postId}", postId);
				return StatusCode(StatusCodes.Status500InternalServerError);
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
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public ActionResult<List<PostReactionToAdd>> GetPostReactions(string postId)
		{
			try
			{
				if (Guid.TryParse(postId, out Guid gPostId))
				{
					if (_postRepository.CheckIfPostExists(gPostId))
					{
						List<PostReaction> reactions = _postReactionRepository.GetPostReactions(gPostId);
						return Ok(reactions);
					}
					else
					{
						return NotFound($"Post: {postId} not found.");
					}
				}
				else
				{
					return BadRequest($"{postId} is not valid Guid.");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during getting reactions of the post: {postId}", postId);
				return StatusCode(StatusCodes.Status500InternalServerError);
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
		public ActionResult<List<PostComment>> GetPostReaction(string postId, string reactionId)
		{
			try
			{
				if (Guid.TryParse(postId, out Guid gPostId) && Guid.TryParse(reactionId, out Guid gReactionId))
				{
					if (_postRepository.CheckIfPostExists(gPostId))
					{
						PostReaction postReaction = _postReactionRepository.GetPostReaction(gReactionId);
						if (postReaction is null)
						{
							return NotFound($"Reaction: {reactionId} not found.");
						}
						else
						{
							return Ok(postReaction);
						}
					}
					else
					{
						return NotFound($"Post: {postId} not found.");
					}
				}
				else
				{
					return BadRequest($"{postId} or {reactionId} is not valid Guid.");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during getting the reaction: postId: {postId}, reactionId: {reactionId}", postId, reactionId);
				return StatusCode(StatusCodes.Status500InternalServerError);
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
			try
			{
				if (Guid.TryParse(reactionId, out Guid gReactionId))
				{
					PostReaction reactionFromRepo = _postReactionRepository.GetPostReaction(gReactionId);
					if (reactionFromRepo == null)
					{
						return NotFound();
					}
					PostReactionToUpdate reactionToPatch = _mapper.Map<PostReactionToUpdate>(reactionFromRepo);
					patchDocument.ApplyTo(reactionToPatch, ModelState);

					if (!TryValidateModel(reactionToPatch))
					{
						return ValidationProblem(ModelState);
					}

					_mapper.Map(reactionToPatch, reactionFromRepo);
					await _postReactionRepository.UpdatePostReactionAsync(reactionFromRepo);
					return NoContent();
				}
				else
				{
					return BadRequest($"{reactionId} is not valid Guid.");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during updating the reaction. Reaction id: {reactionId}", reactionId);
				return StatusCode(StatusCodes.Status500InternalServerError);

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
			try
			{
				if (Guid.TryParse(postId, out Guid gPostId) && Guid.TryParse(fromWho, out Guid gFromWho))
				{
					if (_postRepository.CheckIfPostExists(gPostId) && await _userRepository.CheckIfUserExists(gFromWho))
					{
						PostReaction reactionFromRepo = _postReactionRepository.GetPostReaction(gFromWho, gPostId);
						if (reactionFromRepo == null)
						{
							return NotFound();
						}
						await _postReactionRepository.DeletePostReactionAsync(reactionFromRepo);
						return NoContent();
					}
					else
					{
						return NotFound($"User: {gFromWho} or post {postId} not found.");
					}
				}
				else
				{
					return BadRequest($"{fromWho} or {postId} is not valid Guid.");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during removing the reaction. FromWho id: {fromWho}, post id: {postId}", fromWho, postId);
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}
	}
}
