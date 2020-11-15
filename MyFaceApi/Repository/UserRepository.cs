using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyFaceApi.Data;
using MyFaceApi.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
using MyFaceApi.Repository.Interfaceses;

namespace MyFaceApi.Repository
{
	public class UserRepository : IUserRepository
	{
		private readonly IAppDbContext _appDbContext;
		private readonly ILogger<UserRepository> _logger;

		public UserRepository(IAppDbContext appDbContext,
			ILogger<UserRepository> logger)
		{
			_appDbContext = appDbContext;
			_logger = logger;
		}

		public async Task<User> AddUserAcync(User user)
		{
			_logger.LogDebug("Trying to add user: {user}.", user);
			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}
			try
			{
				var savedUser = await _appDbContext.Users.AddAsync(user);
				await _appDbContext.SaveAsync();
				_logger.LogDebug("User {user} has been addes.", user);
				return savedUser.Entity;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during adding the user.");
				throw;
			}
		}

		public async Task DeleteUserAsync(User user)
		{
			_logger.LogDebug("Trying to remove user: {user}.", user);
			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}
			try
			{
				_appDbContext.Users.Remove(user);
				await _appDbContext.SaveAsync();
				_logger.LogDebug("User {user} has been removed.", user);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during removing the user.");
				throw;
			}
		}

		public async Task<User> GetUserAsync(Guid userId)
		{
			_logger.LogDebug("Trying to get user: {userid}", userId);
			if (userId == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(userId));
			}
			try
			{
				User user = await _appDbContext.Users
					.Include("Posts")
					.Include("Notifications")
					.SingleOrDefaultAsync(x => x.Id == userId);

				if (user != null)
				{
					_logger.LogDebug("User {FirstName} {Lastname} found.", user.FirstName, user.LastName);
					user.Relations = _appDbContext.Relations
					.Where(r => r.FriendId == userId || r.UserId == userId)
					.ToList();
				}
				else
				{
					_logger.LogDebug("User {user} not found.", userId);
				}
				return user;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during getting the user.");
				throw;
			}
		}

		public async Task UpdateUserAsync(User user)
		{
			_logger.LogDebug("Trying to update user: {user}", user);
			try
			{
				await _appDbContext.SaveAsync();
				_logger.LogDebug("User {user} has been updated.", user);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during updating the user.");
				throw;
			}
		}
		public bool CheckIfUserExists(Guid userId)
		{
			_logger.LogDebug("Trying to check if exist user: {userId}", userId);
			if (userId == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(userId));
			}
			try
			{
				bool wasFound = _appDbContext.Users.Any(a => a.Id == userId);
				if (wasFound)
				{
					_logger.LogDebug("User {userId} exist.", userId);
				}
				else
				{
					_logger.LogDebug("User {userId} does not exist.", userId);
				}
				return wasFound;
			}
			catch
			{
				_logger.LogWarning("Something went wrong while searching user: {userId}.", userId);
				throw;
			}
		}
	}
}
