using Microsoft.Extensions.Logging;
using MyFaceApi.Api.Application.Interfaces;
using MyFaceApi.Api.Domain.Entities;
using MyFaceApi.Api.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFaceApi.Api.Application.Services
{
	public class OnlineUsersService : IOnlineUsersService
	{
		private readonly IRepository<OnlineUser> _onlineUsersRepository;
		private readonly ILogger<OnlineUsersService> _logger;
		public OnlineUsersService(IRepository<OnlineUser> onlineUsersRepository,
			ILogger<OnlineUsersService> logger)
		{
			_onlineUsersRepository = onlineUsersRepository;
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
				OnlineUser onlineUser = _onlineUsersRepository.GetById(userId);
				return onlineUser != null;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during checking  if user :{userId) is online.", userId);
				throw;
			}
		}
		public async Task<OnlineUser> AddOnlineUserAsync(OnlineUser onlineUserModel)
		{
			_logger.LogDebug("Trying to add online user.");
			if (onlineUserModel == null)
			{
				throw new ArgumentNullException(nameof(onlineUserModel));
			}
			try
			{
				OnlineUser addedUser = await _onlineUsersRepository.AddAsync(onlineUserModel);
				await _onlineUsersRepository.SaveAsync();
				return addedUser;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during adding the user.");
				throw;
			}

		}
		public OnlineUser GetOnlineUser(string userId)
		{
			_logger.LogDebug("Trying to get online user :{userId).", userId);
			if (string.IsNullOrWhiteSpace(userId))
			{
				throw new ArgumentNullException(nameof(userId));
			}
			try
			{
				return _onlineUsersRepository.GetById(userId);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during getting online user :{userId).", userId);
				throw;
			}
		}
		public async Task RemoveUserAsync(Guid userId)
		{
			_logger.LogDebug("Trying to remove online user.");
			if (userId == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(userId));
			}
			try
			{
				var userToRemove = _onlineUsersRepository.GetById(userId);
				if (userToRemove != null)
				{
					_onlineUsersRepository.Remove(userToRemove);
					await _onlineUsersRepository.SaveAsync();
					_logger.LogDebug("User {userId} has been removed.", userId);
				}

			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during getting removing online user.");
				throw;
			}
		}
		public List<Guid> GetOnlineUsers()
		{
			_logger.LogDebug("Trying to get all online users.");

			try
			{
				var onlineUserStringIds = _onlineUsersRepository.Get()
					.Select(x=>x.Id)
					.ToList();

				List<Guid> usersGuidIds = new List<Guid>();
				onlineUserStringIds.ForEach(x => { usersGuidIds.Add(Guid.Parse(x)); });
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
