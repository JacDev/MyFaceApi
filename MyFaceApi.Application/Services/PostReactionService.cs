using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using MyFaceApi.Api.Application.DtoModels.PostReaction;
using MyFaceApi.Api.Application.Helpers;
using MyFaceApi.Api.Application.Interfaces;
using MyFaceApi.Api.Domain.Entities;
using MyFaceApi.Api.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFaceApi.Api.Application.Services
{
	public class PostReactionService : IPostReactionService
	{
		private readonly IRepository<PostReaction> _postReactionRepository;
		private readonly ILogger<PostReactionService> _logger;
		private readonly IMapper _mapper;

		public PostReactionService(IRepository<PostReaction> postReactionRepository,
			ILogger<PostReactionService> logger,
			IMapper mapper)
		{
			_postReactionRepository = postReactionRepository;
			_logger = logger;
			_mapper = mapper;
		}
		public async Task<PostReactionDto> AddPostReactionAsync(Guid postId, PostReactionToAddDto postReaction)
		{
			_logger.LogDebug("Trying to add reaction {postReaction}.", postReaction);
			if (postReaction is null)
			{
				throw new ArgumentNullException(nameof(PostReaction));
			}
			if (postId == Guid.Empty || postReaction.FromWho == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(Guid));
			}
			try
			{
				PostReaction reactionToAdd = _mapper.Map<PostReaction>(postReaction);
				reactionToAdd.PostId = postId;
				PostReaction addedReaction = await _postReactionRepository.AddAsync(reactionToAdd);
				await _postReactionRepository.SaveAsync();
				return _mapper.Map<PostReactionDto>(addedReaction);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during adding the reaction.");
				throw;
			}
		}
		public async Task DeletePostReactionAsync(Guid postReactionId)
		{
			_logger.LogDebug("Trying to remove reaction: {postReaction}.", postReactionId);
			if (postReactionId == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(postReactionId));
			}
			try
			{
				PostReaction reactionToRemove = _postReactionRepository.GetById(postReactionId);
				if (reactionToRemove != null)
				{
					_postReactionRepository.Remove(reactionToRemove);
					await _postReactionRepository.SaveAsync();
					_logger.LogDebug("Reaction {postReaction} has been removed.", reactionToRemove);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during removing the reaction.");
				throw;
			}
		}
		public async Task DeletePostReactionAsync(Guid userId, Guid postId)
		{
			PostReactionDto reactionToRemove = GetPostReaction(userId, postId);
			await DeletePostReactionAsync(reactionToRemove.Id);
		}
		public PostReactionDto GetPostReaction(Guid reactionId)
		{
			_logger.LogDebug("Trying to get reaction: {reactionId}", reactionId);
			if (reactionId == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(Guid));
			}
			try
			{
				PostReaction reactionFromRepo = _postReactionRepository.GetById(reactionId);
				return _mapper.Map<PostReactionDto>(reactionFromRepo);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during getting the reaction.");
				throw;
			}
		}
		public PostReactionDto GetPostReaction(Guid fromWho, Guid postId)
		{
			_logger.LogDebug("Trying to get reaction: fromWho id: {userId}, post id: {postId}.", fromWho, postId);
			if (fromWho == Guid.Empty || postId == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(Guid));
			}
			try
			{
				List<PostReaction> reactionFromRepo = _postReactionRepository.Get(x => x.FromWho == fromWho && x.PostId == postId)
					.ToList();

				return reactionFromRepo.Count > 0 ?
					_mapper.Map<PostReactionDto>(reactionFromRepo.ElementAt(0)) : null;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during getting the reaction.");
				throw;
			}
		}
		public List<PostReactionDto> GetPostReactions(Guid postId)
		{
			_logger.LogDebug("Trying to get post Reactions: {postId}", postId);
			if (postId == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(Guid));
			}
			try
			{
				List<PostReaction> reactionsFromRepo = _postReactionRepository.Get(x => x.PostId == postId)
					.ToList();

				return _mapper.Map<List<PostReactionDto>>(reactionsFromRepo);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during getting post reactions.");
				throw;
			}
		}
		public async Task<bool> TryUpdatePostReactionAsync(Guid reactionId, JsonPatchDocument<PostReactionToUpdateDto> patchDocument)
		{
			_logger.LogDebug("Trying to update post reaction: {id}.", reactionId);
			try
			{
				PostReaction reactionFromRepo = _postReactionRepository.GetById(reactionId);
				if (reactionFromRepo == null)
				{
					return false;
				}
				PostReactionToUpdateDto reactionToPatch = _mapper.Map<PostReactionToUpdateDto>(reactionFromRepo);
				patchDocument.ApplyTo(reactionToPatch);
				if (!ValidatorHelper.ValidateModel(reactionToPatch))
				{
					return false;
				}

				_mapper.Map(reactionToPatch, reactionFromRepo);
				_postReactionRepository.Update(reactionFromRepo);
				await _postReactionRepository.SaveAsync();
				_logger.LogDebug("Reaction: {id} has been updated.", reactionId);
				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during updating the post reaction.");
				throw;
			}
		}
	}
}
