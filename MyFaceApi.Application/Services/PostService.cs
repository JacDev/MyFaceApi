using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyFaceApi.Api.Application.DtoModels.Post;
using MyFaceApi.Api.Application.FileManagerInterfaces;
using MyFaceApi.Api.Application.Helpers;
using Pagination.Helpers;
using MyFaceApi.Api.Application.Interfaces;
using MyFaceApi.Api.Domain.Entities;
using MyFaceApi.Api.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MyFaceApi.Api.Application.Services
{
	public class PostService : IPostService
	{
		private readonly IRepository<Post> _postRepository;
		private readonly IFriendsRelationService _friendsRelationService;
		private readonly ILogger<PostService> _logger;
		private readonly IMapper _mapper;
		private readonly IImageManager _imageManager;

		public PostService(IRepository<Post> postRepository,
			ILogger<PostService> logger,
			IMapper mapper,
			IFriendsRelationService friendsRelationService,
			IImageManager imageManager)
		{
			_postRepository = postRepository;
			_logger = logger;
			_mapper = mapper;
			_friendsRelationService = friendsRelationService;
			_imageManager = imageManager;
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
				if (post.Picture != null)
				{
					(post.ImagePath, post.ImageFullPath) = await _imageManager.SaveImage(post.Picture);
				}

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
				return wasFound != null;
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
				Post postFromRepo = _postRepository.GetById(postId);

				if (postFromRepo.ImageFullPath != null && File.Exists(postFromRepo.ImageFullPath))
				{
					File.Delete(postFromRepo.ImageFullPath);
				}

				_postRepository.Remove(postFromRepo);
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
				var postsFromRepo = _postRepository.Get(x => x.Id == postId, includeProperties: "PostComments,PostReactions").ToList();
				if (postsFromRepo.Count > 0)
				{
					Post postToReturn = postsFromRepo.ElementAt(0);
					return _mapper.Map<PostDto>(postToReturn);
				}
				return null;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during getting post.");
				throw;
			}
		}
		public PagedList<PostDto> GetUserPosts(Guid userId, PaginationParams paginationParams)
		{
			List<Post> postsFromRepo = _postRepository.Get(x => x.UserId == userId, x => x.OrderByDescending(x => x.WhenAdded), "PostComments,PostReactions")
					.ToList();
			List<PostDto> postsToReturn = _mapper.Map<List<PostDto>>(postsFromRepo);

			return PagedList<PostDto>.Create(postsToReturn,
							paginationParams.PageNumber,
							paginationParams.PageSize,
							(paginationParams.PageNumber - 1) * paginationParams.PageSize + paginationParams.Skip);
		}
		private List<PostDto> GetLatestFriendsPosts(Guid userId, List<Guid> userFriends)
		{
			userFriends.Add(userId);
			List<Post> postsToReturn = _postRepository.Get(x => userFriends.Contains(x.UserId), x => x.OrderByDescending(x => x.WhenAdded), "PostComments,PostReactions")
				.ToList();
			return _mapper.Map<List<PostDto>>(postsToReturn);

		}
		public PagedList<PostDto> GetLatestFriendsPosts(Guid userId, PaginationParams paginationParams)
		{
			_logger.LogDebug("Trying to get user and friends latest posts: {userid}", userId);
			if (userId == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(userId));
			}
			try
			{
				List<Guid> friendsId = _friendsRelationService.GetUserFriendsId(userId);
				List<PostDto> latestPosts = GetLatestFriendsPosts(userId, friendsId);
				return PagedList<PostDto>.Create(latestPosts,
							paginationParams.PageNumber,
							paginationParams.PageSize,
							(paginationParams.PageNumber - 1) * paginationParams.PageSize + paginationParams.Skip);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during getting latest posts.");
				throw;
			}
		}
		public async Task<bool> TryUpdatePostAsync(Guid postId, JsonPatchDocument<PostToUpdateDto> patchDocument)
		{
			_logger.LogDebug("Trying to update post: {post}", postId);
			try
			{
				Post postFromRepo = _postRepository.GetById(postId);
				if (postFromRepo == null)
				{
					return false;
				}
				PostToUpdateDto postToPatch = _mapper.Map<PostToUpdateDto>(postFromRepo);
				patchDocument.ApplyTo(postToPatch);
				if (!ValidatorHelper.ValidateModel(postToPatch))
				{
					return false;
				}

				_mapper.Map(postToPatch, postFromRepo);
				_postRepository.Update(postFromRepo);
				await _postRepository.SaveAsync();
				_logger.LogDebug("Post {post} has been updated.", postId);
				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during updating the post.");
				throw;
			}
		}
		public FileStreamResult StreamImage(string imageName)
		{
			string mime = imageName.Substring(imageName.LastIndexOf('.') + 1);
			return new FileStreamResult(_imageManager.ImageStream(imageName), $"image/{mime}");
		}
	}
}
