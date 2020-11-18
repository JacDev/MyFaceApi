using Microsoft.Extensions.Logging;
using MyFaceApi.Data;
using MyFaceApi.Entities;
using MyFaceApi.Models.CommentModels;
using MyFaceApi.Repository.Interfaceses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFaceApi.Repository
{
	public class PostCommentRepository : IPostCommentRepository
	{
		private readonly IAppDbContext _appDbContext;
		private readonly ILogger<PostCommentRepository> _logger;

		public PostCommentRepository(IAppDbContext appDbContext,
			ILogger<PostCommentRepository> logger)
		{
			_appDbContext = appDbContext;
			_logger = logger;
		}
		public async Task<PostComment> AddCommentAsync(PostComment postComment)
		{
			_logger.LogDebug("Trying to add comment: {postComment} to post.", postComment);
			if (postComment is null)
			{
				throw new ArgumentNullException(nameof(postComment));
			}
			if (postComment.PostId == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(postComment.PostId));
			}
			try
			{
				var addedComment = await _appDbContext.PostComments.AddAsync(postComment);
				await _appDbContext.SaveAsync();
				return addedComment.Entity;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during adding comment to the post.");
				throw;
			}
		}

		public async Task DeleteCommentAsync(PostComment comment)
		{
			_logger.LogDebug("Trying to remove comment: {comment}.", comment);
			if (comment is null)
			{
				throw new ArgumentNullException(nameof(comment));
			}
			try
			{
				_appDbContext.PostComments.Remove(comment);
				await _appDbContext.SaveAsync();
				_logger.LogDebug("Comment {comment} has been removed.", comment);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during removing the comment.");
				throw;
			}
		}
		public List<PostComment> GetComments(Guid postId)
		{
			_logger.LogDebug("Trying to get comments of the post: {postId}.", postId);
			if (postId == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(postId));
			}
			try
			{
				var comments = _appDbContext.PostComments
				.Where(p => p.Id == postId);
				return comments.ToList();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during getting post comments.");
				throw;
			}			
		}
		public PostComment GetComment(Guid commentId)
		{
			_logger.LogDebug("Trying to get comment: {commentId}.", commentId);
			if (commentId == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(commentId));
			}
			try
			{
				var comment = _appDbContext.PostComments.FirstOrDefault(c => c.Id == commentId);
				return comment;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during getting comment.");
				throw;
			}
		}

		public async Task UpdateComment(PostComment postComment)
		{
			_logger.LogDebug("Trying to update comment: {postComment}", postComment);
			try
			{
				await _appDbContext.SaveAsync();
				_logger.LogDebug("Comment {postComment} has been updated.", postComment);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during updating the comment.");
				throw;
			}
		}
	}
}
