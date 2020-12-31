using AutoMapper;
using Microsoft.Extensions.Logging;
using MyFaceApi.Api.Application.DtoModels.FriendsRelation;
using MyFaceApi.Api.Application.DtoModels.User;
using Pagination.Helpers;
using MyFaceApi.Api.Application.Interfaces;
using MyFaceApi.Api.Domain.Entities;
using MyFaceApi.Api.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFaceApi.Api.Application.Services
{
	public class FriendsRelationService : IFriendsRelationService
	{
		private readonly IRepository<FriendsRelation> _friendsRelationRepository;
		private readonly IUserService _userService;
		private readonly ILogger<FriendsRelationService> _logger;
		private readonly IMapper _mapper;
		public FriendsRelationService(IRepository<FriendsRelation> friendsRelationRepository,
			IUserService userService,
			ILogger<FriendsRelationService> logger,
			IMapper mapper)
		{
			_friendsRelationRepository = friendsRelationRepository;
			_logger = logger;
			_userService = userService;
			_mapper = mapper;
		}
		public async Task<FriendsRelationDto> AddRelationAsync(Guid userId, FriendsRelationToAddDto friendRelation)
		{
			_logger.LogDebug("Trying to add friends relation {friendRelation}.", friendRelation);
			if (friendRelation is null)
			{
				throw new ArgumentNullException(nameof(PostReaction));
			}
			if (friendRelation.FriendId == Guid.Empty || userId == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(Guid));
			}
			try
			{
				var relationToAdd = _mapper.Map<FriendsRelation>(friendRelation);
				relationToAdd.UserId = userId;
				var addedRelation = await _friendsRelationRepository.AddAsync(relationToAdd);
				await _friendsRelationRepository.SaveAsync();
				return _mapper.Map<FriendsRelationDto>(addedRelation);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during adding the relation.");
				throw;
			}
		}
		public bool CheckIfAreFriends(Guid firstUser, Guid secondUser)
		{
			_logger.LogDebug("Trying to check users: {firstUser} and {secondUser} relation exist.", firstUser, secondUser);
			if (firstUser == Guid.Empty || secondUser == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(Guid));
			}
			try
			{
				FriendsRelation relation = _friendsRelationRepository.GetById(new { firstUser, secondUser });
				return relation != null;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during checking the relation.");
				throw;
			}
		}
		public async Task DeleteRelationAsync(Guid firstUser, Guid secondUser)
		{
			_logger.LogDebug("Trying to remove users: {firstUser} and {secondUser} relation.", firstUser, secondUser);
			if (firstUser == Guid.Empty || secondUser == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(Guid));
			}
			try
			{
				List<FriendsRelation> relationFromRepo = _friendsRelationRepository.Get(x =>
					x.FriendId == firstUser && x.UserId == secondUser
					|| x.FriendId == secondUser && x.UserId == firstUser)
					.ToList();
				FriendsRelation relationToRemove = null;
				if (relationFromRepo.Count > 0)
				{
					relationToRemove = relationFromRepo.ElementAt(0);
				}
				if (relationToRemove != null)
				{
					_friendsRelationRepository.Remove(relationToRemove);
					await _friendsRelationRepository.SaveAsync();
					_logger.LogDebug("Users relation: {firstUser} and {secondUser} has been removed.", firstUser, secondUser);
				}

			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during removing the relation.");
				throw;
			}
		}
		public FriendsRelationDto GetFriendRelation(Guid firstUser, Guid secondUser)
		{
			_logger.LogDebug("Trying to get users: {firstUser} and {secondUser} relation.", firstUser, secondUser);
			if (firstUser == Guid.Empty || secondUser == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(Guid));
			}
			try
			{
				List<FriendsRelation> relation = _friendsRelationRepository.Get(x =>
					x.FriendId == firstUser && x.UserId == secondUser
					|| x.FriendId == secondUser && x.UserId == firstUser)
					.ToList();

				if (relation.Count > 0)
				{
					return _mapper.Map<FriendsRelationDto>(relation.ElementAt(0));
				}
				return null;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during getting the relation.");
				throw;
			}
		}
		public List<Guid> GetUserFriendsId(Guid userId)
		{
			_logger.LogDebug("Trying to get user: {userId} friends.", userId);
			if (userId == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(Guid));
			}
			try
			{
				List<Guid> friendsIds = _friendsRelationRepository.Get((x => x.UserId == userId || x.FriendId == userId), x => x.OrderByDescending(x => x.SinceWhen))
					.Select(s => (s.FriendId == userId ? s.UserId : s.FriendId))
					.ToList();

				return friendsIds;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during getting the user: {userId} friends.", userId);
				throw;
			}
		}
		public async Task<PagedList<UserDto>> GetUserFriends(Guid userId, PaginationParams paginationParams)
		{
			List<Guid> friendsIds = GetUserFriendsId(userId);
			PagedList<Guid> friendsToTakeFromApi = PagedList<Guid>.Create(friendsIds,
							paginationParams.PageNumber,
							paginationParams.PageSize,
							(paginationParams.PageNumber - 1) * paginationParams.PageSize + paginationParams.Skip);

			List<UserDto> usersToReturn = await _userService.GetUsersAsync(friendsToTakeFromApi);
			return PagedList<UserDto>.CreateNewWithSameParams(friendsToTakeFromApi, usersToReturn);
		}
	}
}
