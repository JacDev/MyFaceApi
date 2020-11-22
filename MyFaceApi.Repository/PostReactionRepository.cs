using Microsoft.Extensions.Logging;
using MyFaceApi.DataAccess.Data;
using MyFaceApi.DataAccess.Entities;
using MyFaceApi.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFaceApi.Repository
{
	public class PostReactionRepository : IPostReactionRepository
	{
		private readonly IAppDbContext _appDbContext;
		private readonly ILogger<PostReactionRepository> _logger;

		public PostReactionRepository(IAppDbContext appDbContext,
			ILogger<PostReactionRepository> logger)
		{
			_appDbContext = appDbContext;
			_logger = logger;
		}

		public async Task<PostReaction> AddPostReactionAsync(PostReaction postReaction)
		{
			_logger.LogDebug("Trying to add reaction {postReaction}.", postReaction);
			if (postReaction is null)
			{
				throw new ArgumentNullException(nameof(PostReaction));
			}
			if (postReaction.PostId == Guid.Empty || postReaction.FromWho == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(Guid));
			}
			try
			{
				var addedReaction = await _appDbContext.PostReactions.AddAsync(postReaction);
				await _appDbContext.SaveAsync();
				return addedReaction.Entity;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during adding the reaction.");
				throw;
			}
		}

		public async Task DeletePostReactionAsync(PostReaction postReaction)
		{
			_logger.LogDebug("Trying to remove reaction: {postReaction}.", postReaction);
			if (postReaction is null)
			{
				throw new ArgumentNullException(nameof(PostReaction));
			}
			try
			{
				_appDbContext.PostReactions.Remove(postReaction);
				await _appDbContext.SaveAsync();
				_logger.LogDebug("Reaction {postReaction} has been removed.", postReaction);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during removing the reaction.");
				throw;
			}
		}

		public PostReaction GetPostReaction(Guid reactionId)
		{
			_logger.LogDebug("Trying to get reaction: {reactionId}", reactionId);
			if (reactionId == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(Guid));
			}
			try
			{
				var reactions =  _appDbContext.PostReactions.FirstOrDefault(p => p.Id == reactionId);

				return reactions;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during getting the reaction.");
				throw;
			}
		}

		public List<PostReaction> GetPostReactions(Guid postId)
		{
			_logger.LogDebug("Trying to get post Reactions: {postId}", postId);
			if (postId == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(Guid));
			}
			try
			{
				List<PostReaction> reactions = _appDbContext.PostReactions
					.Where(p => p.Id == postId)
					.ToList();

				return reactions;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during getting post reactions.");
				throw;
			}
		}

		public async Task UpdatePostReactionAsync(PostReaction postReaction)
		{
			_logger.LogDebug("Trying to update reaction: {postReaction}", postReaction);
			try
			{
				await _appDbContext.SaveAsync();
				_logger.LogDebug("Reaction {postReaction} has been updated.", postReaction);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during updating the reaction.");
				throw;
			}
		}
	}
}
