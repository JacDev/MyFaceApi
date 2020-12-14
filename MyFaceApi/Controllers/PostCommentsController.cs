using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyFaceApi.Api.Application.DtoModels.Comment;
using MyFaceApi.Api.Application.Interfaces;
using Pagination.Helpers;
using Pagination.DtoModels;
using Pagination.Extensions;
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
		private readonly IPostService _postService;
		private readonly IUserService _userService;
		private readonly IPostCommentService _postCommentService;
		private readonly IMapper _mapper;
		public PostCommentsController(ILogger<PostCommentsController> logger,
			IPostService postService,
			IUserService userService,
			IMapper mapper,
			IPostCommentService postCommentService)
		{
			_logger = logger;
			_postService = postService;
			_userService = userService;
			_mapper = mapper;
			_postCommentService = postCommentService;
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
		[AllowAnonymous]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<CommentDto>> AddComment(string userId, string postId, [FromBody] CommentToAddDto postComment)
		{
			if (Guid.TryParse(postId, out Guid gPostId) && Guid.TryParse(userId, out Guid gUserId))
			{
				try
				{
					if (_postService.CheckIfPostExists(gPostId) && await _userService.CheckIfUserExists(gUserId))
					{
						CommentDto addedComment = await _postCommentService.AddCommentAsync(gPostId, postComment);
						return CreatedAtRoute("GetComment",
							new
							{
								userId,
								postId = addedComment.PostId,
								commentId = addedComment.Id
							},
							addedComment);
					}
					else
					{
						return NotFound($"User: {userId} or post {postId} not found.");
					}
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error occured during adding the comment. Post id: {postId}", postId);
					return StatusCode(StatusCodes.Status500InternalServerError);
				}
			}
			else
			{
				return BadRequest($"{userId} or {postId} is not valid Guid.");
			}
		}
		/// <summary>
		/// Return the found post comments
		/// </summary>
		/// <param name="postId">Post guid as a string </param>
		/// <param name="paginationParams"></param>
		/// <returns>Found post comments</returns>
		/// <response code="200"> Returns the found post comments</response>
		/// <response code="400"> If parameter is not a valid guid</response>    
		/// <response code="404"> If post not found</response>   
		/// <response code="500"> If internal error occured</response>
		[HttpGet(Name = "GetComments")]
		[AllowAnonymous]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public ActionResult<CollectionWithPaginationData<CommentDto>> GetComments(string postId, [FromQuery] PaginationParams paginationParams)
		{
			try
			{
				if (Guid.TryParse(postId, out Guid gPostId))
				{
					if (_postService.CheckIfPostExists(gPostId))
					{
						PagedList<CommentDto> commentsToReturn = _postCommentService.GetPostComments(gPostId, paginationParams);
						return Ok(this.CreateCollectionWithPagination(commentsToReturn, paginationParams, "GetPosts"));
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
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public ActionResult<List<CommentDto>> GetComment(string postId, string commentId)
		{
			try
			{
				if (Guid.TryParse(postId, out Guid gPostId) && Guid.TryParse(commentId, out Guid gCommentId))
				{
					if (_postService.CheckIfPostExists(gPostId))
					{
						return Ok(_postCommentService.GetComment(gCommentId));
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
		/// <response code="500"> If internal error occured</response>
		[HttpPatch("{commentId}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult> PartiallyUpdateComment(string commentId, JsonPatchDocument<CommentToUpdateDto> patchDocument)
		{
			try
			{
				if (Guid.TryParse(commentId, out Guid gCommentId))
				{
					if (await _postCommentService.TryUpdatePostCommentAsync(gCommentId, patchDocument))
					{
						return NoContent();
					}
					else
					{
						return BadRequest();
					}
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
		/// <response code="500"> If internal error occured</response>
		[HttpDelete("{commentId}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult> DeleteComment(string commentId)
		{
			try
			{
				if (Guid.TryParse(commentId, out Guid gCommentId))
				{
					await _postCommentService.DeleteCommentAsync(gCommentId);
					return NoContent();
				}
				else
				{
					return BadRequest($"{commentId} is not valid guid.");
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
