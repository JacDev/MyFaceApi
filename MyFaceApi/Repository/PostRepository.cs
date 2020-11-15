using Microsoft.Extensions.Logging;
using MyFaceApi.Data;
using MyFaceApi.Entities;
using MyFaceApi.Repository.Interfaceses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFaceApi.Repository
{
	public class PostRepository : IPostRepository
	{
		private readonly IAppDbContext _appDbContext;
		private readonly ILogger<PostRepository> _logger;

		public PostRepository(IAppDbContext appDbContext,
			ILogger<PostRepository> logger)
		{
			_appDbContext = appDbContext;
			_logger = logger;
		}
		public async Task<Post> AddPostAsync(Post post)
		{
			_logger.LogDebug("Trying to add post {post}.", post);
			if (post == null)
			{
				throw new ArgumentNullException(nameof(post));
			}
			if (post.UserId == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(post.UserId));
			}
			try
			{
				var addedPost = await _appDbContext.Posts.AddAsync(post);
				await _appDbContext.SaveAsync();
				return addedPost.Entity;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during adding the post.");
				throw;
			}
		}
		public async Task DeletePostAsync(Post post)
		{
			_logger.LogDebug("Trying to remove post: {post}.", post);
			if (post == null)
			{
				throw new ArgumentNullException(nameof(post));
			}
			try
			{
				_appDbContext.Posts.Remove(post);
				await _appDbContext.SaveAsync();
				_logger.LogDebug("Post {post} has been removed.", post);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during removing the post.");
				throw;
			}
		}
		public Post GetPost(Guid postId)
		{
			_logger.LogDebug("Trying to get the post: {userid}", postId);
			if (postId == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(postId));
			}
			try
			{
				Post postToReturn = _appDbContext.Posts
				.FirstOrDefault(s => s.Id == postId);
				return postToReturn;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during getting post.");
				throw;
			}
		}
		public List<Post> GetUserPosts(Guid userId)
		{
			_logger.LogDebug("Trying to get user posts: {userid}", userId);
			if (userId == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(userId));
			}
			try
			{
				List<Post> posts = new List<Post>();
				posts = _appDbContext.Posts
					.Where(s => s.UserId == userId)
					.ToList();
				return posts;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during getting user posts.");
				throw;
			}
		}

		public async Task UpdatePostAsync(Post post)
		{
			_logger.LogDebug("Trying to update post: {post}", post);
			try
			{
				await _appDbContext.SaveAsync();
				_logger.LogDebug("Post {post} has been updated.", post);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during updating the post.");
				throw;
			}
		}
	}
}
