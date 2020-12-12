using AutoMapper;
using Microsoft.Extensions.Logging;
using MyFaceApi.Api.Application.DtoModels.Post;
using MyFaceApi.Api.Application.Interfaces;
using MyFaceApi.Api.Domain.Entities;
using MyFaceApi.Api.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFaceApi.Api.Application.Services
{
	public class PostService : IPostService
	{
		private readonly IRepository<Post> _postRepository;
		private readonly ILogger<PostService> _logger;
		private readonly IMapper _mapper;

		public PostService(IRepository<Post> postRepository,
			ILogger<PostService> logger,
			IMapper mapper)
		{
			_postRepository = postRepository;
			_logger = logger;
			_mapper = mapper;
		}
		public async Task<PostDto> AddPostAsync(Guid userId, PostToAddDto post)
		{
			_logger.LogDebug("Trying to add post {post}.", post);
			if (post == null)
			{
				throw new ArgumentNullException(nameof(post));
			}
			if (userId == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(userId));
			}
			try
			{
				Post postToAdd = _mapper.Map<Post>(post);
				postToAdd.UserId = userId;

				Post addedPost = await _postRepository.AddAsync(postToAdd);
				await _postRepository.SaveAsync();

				return _mapper.Map<PostDto>(addedPost);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during adding the post.");
				throw;
			}
		}
		public bool CheckIfPostExists(Guid postId)
		{
			if (postId == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(postId));
			}
			_logger.LogDebug($"Trying to check if the post: {postId} exist");
			try
			{
				var wasFound = _postRepository.GetById(postId);
				if (wasFound is null)
				{
					_logger.LogTrace("Post {postId} does not exist.", postId);
				}
				else
				{
					_logger.LogTrace("Post {postId} exist.", postId);
				}
				return wasFound!=null;
			}
			catch
			{
				_logger.LogWarning($"Something went wrong while searching post: {postId}.");
				throw;
			}
		}
		public async Task DeletePostAsync(Guid postId)
		{
			_logger.LogDebug("Trying to remove post: {postId}.", postId);
			if (postId == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(postId));
			}
			try
			{
				_postRepository.Remove(postId);
				await _postRepository.SaveAsync();
				_logger.LogDebug("Post {postId} has been removed.", postId);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during removing the post.");
				throw;
			}
		}
		public PostDto GetPost(Guid postId)
		{
			_logger.LogDebug("Trying to get the post: {userid}", postId);
			if (postId == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(postId));
			}
			try
			{
				Post postFromRepo = _postRepository.GetById(postId);
				return _mapper.Map<PostDto>(postFromRepo);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during getting post.");
				throw;
			}
		}
		public List<PostDto> GetUserPosts(Guid userId)
		{
			_logger.LogDebug("Trying to get the user posts: {userid}", userId);
			if (userId == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(userId));
			}
			try
			{
				List<Post> postsToReturn = _postRepository.Get(x => x.UserId == userId, x => x.OrderByDescending(x => x.WhenAdded), "PostComments,PostReactions")
					.ToList();

				return _mapper.Map<List<PostDto>>(postsToReturn);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during getting the user posts.");
				throw;
			}
		}
		public List<PostDto> GetLatestFriendsPosts(Guid userId, List<Guid> userFriends)
		{
			_logger.LogDebug("Trying to get user and friends latest posts: {userid}", userId);
			if (userId == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(userId));
			}
			try
			{
				userFriends.Add(userId);
				List<Post> postsToReturn = _postRepository.Get(x => userFriends.Contains(x.UserId), x => x.OrderByDescending(x => x.WhenAdded), "PostComments,PostReactions")
					.ToList();

				return _mapper.Map<List<PostDto>>(postsToReturn);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during getting latest posts.");
				throw;
			}
		}
		public async Task UpdatePostAsync(Post post)
		{
			_logger.LogDebug("Trying to update post: {post}", post);
			try
			{
				_postRepository.Update(post);
				await _postRepository.SaveAsync();
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
