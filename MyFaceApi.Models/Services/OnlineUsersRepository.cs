using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyFaceApi.Api.DataAccess.Data;
using MyFaceApi.Api.DataAccess.Entities;
using MyFaceApi.Api.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFaceApi.Api.Repository.Repositories
{
	public class OnlineUsersRepository : IOnlineUsersRepository
	{
		private readonly IOnlineUsersDbContext _onlineUsersDbContext;
		private readonly ILogger<OnlineUsersRepository> _logger;
		public OnlineUsersRepository(IOnlineUsersDbContext mVCDbContext,
			ILogger<OnlineUsersRepository> logger)
		{
			_onlineUsersDbContext = mVCDbContext;
			_logger = logger;
		}
		public bool IsUserOnline(string userId)
		{
			_logger.LogDebug("Trying to check if user :{userId) is online.", userId);
			if (string.IsNullOrWhiteSpace(userId))
			{
				throw new ArgumentNullException(nameof(userId));
			}
			try
			{
				return _onlineUsersDbContext.OnlineUsers.Any(x => x.Id == userId);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during checking  if user :{userId) is online.", userId);
				throw;
			}
		}
		public async Task AddOnlineUser(OnlineUserModel onlineUserModel)
		{
			_logger.LogDebug("Trying to add online user.");
			if (onlineUserModel == null)
			{
				throw new ArgumentNullException(nameof(onlineUserModel));
			}
			try
			{
				_onlineUsersDbContext.OnlineUsers.Add(onlineUserModel);
				await _onlineUsersDbContext.SaveAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during adding user.");
				throw;
			}

		}
		public OnlineUserModel GetOnlineUser(string userId)
		{
			_logger.LogDebug("Trying to get online user :{userId).", userId);
			if (string.IsNullOrWhiteSpace(userId))
			{
				throw new ArgumentNullException(nameof(userId));
			}
			try
			{
				return _onlineUsersDbContext.OnlineUsers.FirstOrDefault(u => u.Id == userId); ;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during getting online user :{userId).", userId);
				throw;
			}
		}
		public async Task RemoveUser(OnlineUserModel user)
		{
			_logger.LogDebug("Trying to remove online user.");
			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}
			try
			{
				_onlineUsersDbContext.OnlineUsers.Remove(user);
				await _onlineUsersDbContext.SaveAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during getting removing online user.");
				throw;
			}
		}
		public async Task<List<Guid>> GetOnlineUsersAsync()
		{
			_logger.LogDebug("Trying to get all online users.");

			try
			{
				var usersStringIds = await _onlineUsersDbContext.OnlineUsers
					.Select(x =>x.Id)
					.ToListAsync();

				List<Guid> usersGuidIds = new List<Guid>();
				usersStringIds.ForEach(x => { usersGuidIds.Add(Guid.Parse(x)); });
				return usersGuidIds;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during getting online usera.");
				throw;
			}
		}
	}
}
