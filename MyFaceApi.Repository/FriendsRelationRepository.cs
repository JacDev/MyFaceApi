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
	public class FriendsRelationRepository : IFriendsRelationRepository
	{
		private readonly IAppDbContext _appDbContext;
		private readonly ILogger<FriendsRelationRepository> _logger;
		public FriendsRelationRepository(IAppDbContext appDbContext,
			ILogger<FriendsRelationRepository> logger)
		{
			_appDbContext = appDbContext;
			_logger = logger;
		}

		public async Task<FriendsRelation> AddRelationAsync(FriendsRelation friendRelation)
		{
			_logger.LogDebug("Trying to add friends relation {friendRelation}.", friendRelation);
			if (friendRelation is null)
			{
				throw new ArgumentNullException(nameof(PostReaction));
			}
			if (friendRelation.FriendId == Guid.Empty || friendRelation.UserId == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(Guid));
			}
			try
			{
				var addedRelation = await _appDbContext.Relations.AddAsync(friendRelation);
				await _appDbContext.SaveAsync();
				return addedRelation.Entity;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during adding the relation.");
				throw;
			}
		}

		public bool CheckIfFriends(Guid firstUser, Guid secondUser)
		{
			_logger.LogDebug("Trying to check the users: {firstUser} and {secondUser} relation exist.", firstUser, secondUser);
			if (firstUser == Guid.Empty || secondUser == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(Guid));
			}
			try
			{
				return _appDbContext.Relations.Any(
				s => s.UserId == firstUser && s.FriendId == secondUser
				|| s.UserId == secondUser && s.FriendId == firstUser);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during checking the relation.");
				throw;
			}
		}

		public async Task DeleteRelationAsync(FriendsRelation friendsRelation)
		{
			_logger.LogDebug("Trying to remove relation: {friendRelation}.", friendsRelation);
			if (friendsRelation is null)
			{
				throw new ArgumentNullException(nameof(PostReaction));
			}
			try
			{
				_appDbContext.Relations.Remove(friendsRelation);
				await _appDbContext.SaveAsync();
				_logger.LogDebug("Relation {friendRelation} has been removed.", friendsRelation);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during removing the relation.");
				throw;
			}
		}

		public FriendsRelation GetFriendRelation(Guid firstUser, Guid secondUser)
		{
			_logger.LogDebug("Trying to get users: {firstUser} and {secondUser} relation.", firstUser, secondUser);
			if (firstUser == Guid.Empty || secondUser == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(Guid));
			}
			try
			{
				var relation = _appDbContext.Relations.FirstOrDefault(
				s => s.UserId == firstUser && s.FriendId == secondUser
				|| s.UserId == secondUser && s.FriendId == firstUser);

				return relation;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during getting the relation.");
				throw;
			}
		}

		public List<FriendsRelation> GetUserRelationships(Guid userId)
		{
			_logger.LogDebug("Trying to get user: {userId} relations.", userId);
			if (userId == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(Guid));
			}
			try
			{
				var relations = _appDbContext.Relations.Where(s => s.UserId == userId || s.FriendId == userId).ToList();
				return relations;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during getting the user: {userId} relations.", userId);
				throw;
			}
		}
	}
}
