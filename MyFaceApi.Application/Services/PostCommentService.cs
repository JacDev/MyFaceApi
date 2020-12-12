using AutoMapper;
using Microsoft.Extensions.Logging;
using MyFaceApi.Api.Application.DtoModels.Comment;
using MyFaceApi.Api.Application.Interfaces;
using MyFaceApi.Api.Domain.Entities;
using MyFaceApi.Api.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFaceApi.Api.Application.Services
{
	public class PostCommentService : IPostCommentService
	{
		private readonly IRepository<PostComment> _postCommentRepository;
		private readonly ILogger<PostCommentService> _logger;
		private readonly IMapper _mapper;

		public PostCommentService(IRepository<PostComment> postCommentRepository,
			ILogger<PostCommentService> logger,
			IMapper mapper)
		{
			_postCommentRepository = postCommentRepository;
			_logger = logger;
			_mapper = mapper;
		}
		public async Task<CommentDto> AddCommentAsync(Guid postId, CommentToAddDto postComment)
		{
			_logger.LogDebug("Trying to add comment: {postComment} to post.", postComment);
			if (postComment is null)
			{
				throw new ArgumentNullException(nameof(postComment));
			}
			if (postId == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(postId));
			}
			try
			{
				PostComment commentToAdd = _mapper.Map<PostComment>(postComment);
				commentToAdd.PostId = postId;
				PostComment addedComment = await _postCommentRepository.AddAsync(commentToAdd);
				await _postCommentRepository.SaveAsync();
				return _mapper.Map<CommentDto>(addedComment);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during adding comment to the post.");
				throw;
			}
		}
		public async Task DeleteCommentAsync(Guid commentId)
		{
			_logger.LogDebug("Trying to remove comment: {comment}.", commentId);
			if (commentId == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(commentId));
			}
			try
			{
				PostComment commentToRemove = _postCommentRepository.GetById(commentId);
				if (commentToRemove != null)
				{
					_postCommentRepository.Remove(commentToRemove);
					await _postCommentRepository.SaveAsync();
					_logger.LogDebug("Comment {comment} has been removed.", commentToRemove);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during removing the comment.");
				throw;
			}
		}
		public List<CommentDto> GetPostComments(Guid postId)
		{
			_logger.LogDebug("Trying to get comments of the post: {postId}.", postId);
			if (postId == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(postId));
			}
			try
			{
				List<PostComment> commentsFromRepo = _postCommentRepository.Get(x => x.PostId == postId)
					.ToList();
	
				return _mapper.Map<List<CommentDto>>(commentsFromRepo);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during getting post comments.");
				throw;
			}			
		}
		public CommentDto GetComment(Guid commentId)
		{
			_logger.LogDebug("Trying to get comment: {commentId}.", commentId);
			if (commentId == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(commentId));
			}
			try
			{
				PostComment commentFromRepo = _postCommentRepository.GetById(commentId);
				return _mapper.Map<CommentDto>(commentFromRepo);
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
				_postCommentRepository.Update(postComment);
				await _postCommentRepository.SaveAsync();
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
