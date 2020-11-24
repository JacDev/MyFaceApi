using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyFaceApi.Api.DataAccess.Entities;
using MyFaceApi.Api.Models.CommentModels;
using MyFaceApi.Api.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyFaceApi.Api.Controllers
{
	[Route("api/users/{userId}/posts/{postId}/comments")]
	[ApiController]
	public class PostCommentsController : ControllerBase
	{
		private readonly ILogger<PostCommentsController> _logger;
		private readonly IPostRepository _postRepository;
		private readonly IUserRepository _userRepository;
		private readonly IPostCommentRepository _postCommentRepository;
		private readonly IMapper _mapper;
		public PostCommentsController(ILogger<PostCommentsController> logger,
			IPostRepository postRepository,
			IUserRepository userRepository,
			IMapper mapper, 
			IPostCommentRepository postCommentRepository)
		{
			_logger = logger;
			_postRepository = postRepository;
			_userRepository = userRepository;
			_mapper = mapper;
			_postCommentRepository = postCommentRepository;
			_logger.LogTrace("PostCommentController created");
		}
		/// <summary>
		/// Add post to database
		/// </summary>
		/// <param name="userId">User guid as a string </param>
		/// <param name="postId">Post guid as a string</param>
		/// <param name="postComment">Comment to add of a type CommentToAdd</param>
		/// <returns>Added post</returns>
		/// <response code="201"> Return created comment</response>
		/// <response code="400"> If parameter is not a valid guid</response>    
		/// <response code="404"> If user or post not found</response>   
		/// <response code="500"> If internal error occured</response>
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<PostComment>> AddComment(string userId, string postId, [FromBody] CommentToAdd postComment)
		{
			try
			{
				if (Guid.TryParse(postId, out Guid gPostId) && Guid.TryParse(userId, out Guid gUserId))
				{
					if (_postRepository.CheckIfPostExists(gPostId) && _userRepository.CheckIfUserExists(gUserId))
					{
						PostComment commentEntity = _mapper.Map<PostComment>(postComment);
						commentEntity = await _postCommentRepository.AddCommentAsync(commentEntity);

						return CreatedAtRoute("GetComment",
							new { userId, postId = commentEntity.PostId, 
								commentId = commentEntity.Id },
							commentEntity);
					}
					else
					{
						return NotFound($"User: {userId} or post {postId} not found.");
					}
				}
				else
				{
					return BadRequest($"{userId} or {postId} is not valid Guid.");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during adding the comment. Post id: {postId}", postId);
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}
		/// <summary>
		/// Return the found post comments
		/// </summary>
		/// <param name="postId">Post guid as a string </param>
		/// <returns>Found post comments</returns>
		/// <response code="200"> Returns the found post comments</response>
		/// <response code="400"> If parameter is not a valid guid</response>    
		/// <response code="404"> If post not found</response>   
		/// <response code="500"> If internal error occured</response>
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public ActionResult<List<PostComment>> GetComments(string postId)
		{
			try
			{
				if (Guid.TryParse(postId, out Guid gPostId))
				{
					if (_postRepository.CheckIfPostExists(gPostId))
					{
						List<PostComment> comments = _postCommentRepository.GetComments(gPostId);
						return Ok(comments);
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
				_logger.LogError(ex, "Error occured during getting comments of the post: {postId}", postId);
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}
		/// <summary>
		/// Return the found comment
		/// </summary>
		/// <param name="postId">Post guid as a string </param>
		/// <param name="commentId">Comment guid as a string </param>
		/// <returns>Found comment</returns>
		/// <response code="200"> Returns the found comments</response>
		/// <response code="400"> If parameter is not a valid guid</response>    
		/// <response code="404"> If post or comment not found</response>   
		/// <response code="500"> If internal error occured</response>
		[HttpGet("{commentId}", Name = "GetComment")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public ActionResult<List<PostComment>> GetComment(string postId, string commentId)
		{
			try
			{
				if (Guid.TryParse(postId, out Guid gPostId) && Guid.TryParse(commentId, out Guid gCommentId))
				{
					if (_postRepository.CheckIfPostExists(gPostId))
					{
						PostComment comment = _postCommentRepository.GetComment(gCommentId);
						if(comment is null)
						{
							return NotFound($"Comment: {commentId} not found.");
						}
						else
						{
							return Ok(comment);
						}
					}
					else
					{
						return NotFound($"Post: {postId} not found.");
					}
				}
				else
				{
					return BadRequest($"{postId} or {commentId} is not valid Guid.");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during getting comment: postId: {postId}, commentId: {commentId}", postId, commentId);
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}
		/// <summary>
		/// Update comment in database
		/// </summary>
		/// <param name="commentId"></param>
		/// <param name="patchDocument"></param>
		/// <returns>
		/// Status 204 no content if the comment has been updated
		/// </returns>
		/// <response code="204"> No content if the comment has been updated</response>
		/// <response code="400"> If the comment is not valid</response>    
		/// <response code="404"> If the comment not found</response>
		/// <response code="500"> If internal error occured</response>
		[HttpPatch("{commentId}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult> PartiallyUpdateComment(string commentId, JsonPatchDocument<CommentToUpdate> patchDocument)
		{
			try
			{
				if (Guid.TryParse(commentId, out Guid gCommentId))
				{
					PostComment commentFromRepo = _postCommentRepository.GetComment(gCommentId);
					if (commentFromRepo == null)
					{
						return NotFound();
					}
					CommentToUpdate commentToPatch = _mapper.Map<CommentToUpdate>(commentFromRepo);
					patchDocument.ApplyTo(commentToPatch, ModelState);

					if (!TryValidateModel(commentToPatch))
					{
						return ValidationProblem(ModelState);
					}

					_mapper.Map(commentToPatch, commentFromRepo);
					await _postCommentRepository.UpdateComment(commentFromRepo);
					return NoContent();
				}
				else
				{
					return BadRequest($"{commentId} is not valid Guid.");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during updating the comment. Comment id: {commentId}", commentId);
				return StatusCode(StatusCodes.Status500InternalServerError);

			}
		}
		/// <summary>
		/// Remove a comment from the database
		/// </summary>
		/// <param name="commentId"></param>
		/// <returns>
		/// Status 204 no content if the comment has been removed
		/// </returns>
		/// <response code="204"> No content if the comment has been updated</response>
		/// <response code="400"> If the post is not valid</response>    
		/// <response code="404"> If the post not found</response>
		/// <response code="500"> If internal error occured</response>
		[HttpDelete("{commentId}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]

		public async Task<ActionResult> DeleteComment(string commentId)
		{
			try
			{
				if (Guid.TryParse(commentId, out Guid gCommentId))
				{
					PostComment commentFromRepo = _postCommentRepository.GetComment(gCommentId);
					if (commentFromRepo == null)
					{
						return NotFound();
					}
					await _postCommentRepository.DeleteCommentAsync(commentFromRepo);
					return NoContent();
				}
				else
				{
					return BadRequest($"{commentId} is not valid Guid.");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during removing the comment. Coment id: {postId}", commentId);
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}
	}
}
