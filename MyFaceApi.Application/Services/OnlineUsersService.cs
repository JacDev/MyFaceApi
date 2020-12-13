using AutoMapper;
using Microsoft.Extensions.Logging;
using MyFaceApi.Api.Application.DtoModels.User;
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
	public class OnlineUsersService : IOnlineUsersService
	{
		private readonly IRepository<OnlineUser> _onlineUsersRepository;
		private readonly IFriendsRelationService _friendsRelationService;
		private readonly IUserService _userService;
		private readonly ILogger<OnlineUsersService> _logger;
		private readonly IMapper _mapper;
		public OnlineUsersService(IRepository<OnlineUser> onlineUsersRepository,
			ILogger<OnlineUsersService> logger,
			IUserService userService)
		{
			_onlineUsersRepository = onlineUsersRepository;
			_logger = logger;
			_userService = userService;
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
		public async Task<OnlineUserDto> AddOnlineUserAsync(OnlineUserDto onlineUserToAdd)
		{
			_logger.LogDebug("Trying to add online user.");
			if (onlineUserToAdd == null)
			{
				throw new ArgumentNullException(nameof(onlineUserToAdd));
			}
			try
			{
				OnlineUser userToAdd = _mapper.Map<OnlineUser>(onlineUserToAdd);
				OnlineUser addedUser = await _onlineUsersRepository.AddAsync(userToAdd);
				await _onlineUsersRepository.SaveAsync();
				return _mapper.Map<OnlineUserDto>(addedUser);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during adding the user.");
				throw;
			}
		}
		public OnlineUserDto GetOnlineUser(string userId)
		{
			_logger.LogDebug("Trying to get online user :{userId).", userId);
			if (string.IsNullOrWhiteSpace(userId))
			{
				throw new ArgumentNullException(nameof(userId));
			}
			try
			{
				OnlineUser userToReturn = _onlineUsersRepository.GetById(userId);
				return _mapper.Map<OnlineUserDto>(userToReturn);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during getting online user :{userId).", userId);
				throw;
			}
		}
		public async Task<PagedList<UserDto>> GetOnlineFriends(Guid userId, PaginationParams paginationParams)
		{
			List<Guid> onlineUsersIds = GetOnlineUsers();
			List<Guid> userFriendsIds = _friendsRelationService.GetUserFriendsId(userId);

			List<Guid> onlineFriendsIds = onlineUsersIds.Intersect(userFriendsIds).ToList();
			PagedList<Guid> onlineFriendsPages = PagedList<Guid>.Create(onlineFriendsIds,
							paginationParams.PageNumber,
							paginationParams.PageSize,
							(paginationParams.PageNumber - 1) * paginationParams.PageSize + paginationParams.Skip);

			List<UserDto> onlineFriendsFromApi = await _userService.GetUsersAsync(onlineFriendsPages);
			return PagedList<UserDto>.CreateNewWithSameParams(onlineFriendsPages, onlineFriendsFromApi);
		}
		public async Task RemoveUserAsync(string userId)
		{
			_logger.LogDebug("Trying to remove online user.");
			if (string.IsNullOrWhiteSpace(userId))
			{
				throw new ArgumentNullException(nameof(userId));
			}
			try
			{
				OnlineUser userToRemove = _onlineUsersRepository.GetById(userId);
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
		private List<Guid> GetOnlineUsers()
		{
			_logger.LogDebug("Trying to get all online users.");

			try
			{
				List<string> onlineUserStringIds = _onlineUsersRepository.Get()
					.Select(x => x.Id)
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
