﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyFaceApi.Api.DataAccess.Entities;
using MyFaceApi.Api.DataAccess.ModelsBasicInfo;
using MyFaceApi.Api.DboModels;
using MyFaceApi.Api.Extensions;
using MyFaceApi.Api.Helpers;
using MyFaceApi.Api.Models.FriendsRelationModels;
using MyFaceApi.Api.Repository.Helpers;
using MyFaceApi.Api.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyFaceApi.Api.Controllers
{
	[Route("api/users/{userId}/friends")]
	[ApiController]
	public class FriendsRelationsController : ControllerBase
	{
		private readonly ILogger<FriendsRelationsController> _logger;
		private readonly IFriendsRelationRepository _relationRepository;
		private readonly IMapper _mapper;
		private readonly IUserRepository _userRepository;

		public FriendsRelationsController(ILogger<FriendsRelationsController> logger,
			IFriendsRelationRepository relationRepository,
			IMapper mapper,
			IUserRepository userRepository)
		{
			_logger = logger ??
				throw new ArgumentNullException(nameof(logger));
			_relationRepository = relationRepository ??
				throw new ArgumentNullException(nameof(relationRepository));
			_mapper = mapper ??
				throw new ArgumentNullException(nameof(mapper));
			_userRepository = userRepository ??
				throw new ArgumentNullException(nameof(userRepository));
		}
		[HttpGet("{friendId}", Name = "GetRelation")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public ActionResult<FriendsRelation> GetRelation(string userId, string friendId)
		{
			try
			{
				if (Guid.TryParse(userId, out Guid gUserId) && Guid.TryParse(friendId, out Guid gFriendId))
				{
					FriendsRelation friendsRelation = _relationRepository.GetFriendRelation(gUserId, gFriendId);
					return Ok(friendsRelation);
				}
				else
				{
					return BadRequest($"{userId} or {friendId} is not valid Guid.");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during getting the friends relation. Users id: {userId} and {friendId}", userId, friendId);
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}
		[AllowAnonymous]
		[HttpGet(Name = "GetFriends")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<CollectionWithPaginationData<BasicUserData>>> GetUserFriends(string userId, [FromQuery] PaginationParams paginationParams)
		{
			try
			{
				if (Guid.TryParse(userId, out Guid gUserId))
				{
					paginationParams.Skip = (paginationParams.PageNumber - 1) * paginationParams.PageSize;
					if (await _userRepository.CheckIfUserExists(gUserId))
					{
						PagedList<Guid> userFriendsId = _relationRepository.GetUserFriends(gUserId, paginationParams);
						List<BasicUserData> usersToReturn = await _userRepository.GetUsersAsync(userFriendsId);

						PagedList<BasicUserData> friendsToReturn = PagedList<BasicUserData>.Create(usersToReturn,
							paginationParams.PageNumber,
							paginationParams.PageSize,
							0);
						friendsToReturn.TotalCount = userFriendsId.TotalCount;
						friendsToReturn.TotalPages = userFriendsId.TotalPages;

						friendsToReturn.PreviousPageLink = friendsToReturn.HasPrevious ?
							this.CreateMessagesResourceUriWithPaginationParams(paginationParams, ResourceUriType.PreviousPage, "GetFriends") : null;

						friendsToReturn.NextPageLink = friendsToReturn.HasNext ?
							this.CreateMessagesResourceUriWithPaginationParams(paginationParams, ResourceUriType.NextPage, "GetFriends") : null;

						PaginationMetadata pagination = PaginationHelper.CreatePaginationMetadata<BasicUserData>(friendsToReturn);

						return Ok(new CollectionWithPaginationData<BasicUserData> { PaginationMetadata = pagination, Collection = friendsToReturn });
					}
					else
					{
						return NotFound($"User: {userId} not found.");
					}
				}
				else
				{
					return BadRequest($"{userId} is not valid guid.");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during getting the user relations. User id: {user}", userId);
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<FriendsRelation>> AddRelation(string userId, FriendsRelationToAdd relationToAdd)
		{
			try
			{
				if (Guid.TryParse(userId, out Guid gUserId))
				{
					if (await _userRepository.CheckIfUserExists(gUserId) && await _userRepository.CheckIfUserExists(relationToAdd.FriendId))
					{
						FriendsRelation friendsRelationEntity = _mapper.Map<FriendsRelation>(relationToAdd);
						friendsRelationEntity.UserId = gUserId;
						friendsRelationEntity = await _relationRepository.AddRelationAsync(friendsRelationEntity);

						return CreatedAtRoute("GetRelation",
							new { userId, friendId = friendsRelationEntity.FriendId },
							friendsRelationEntity);
					}
					else
					{
						return NotFound($"User: {userId} or friend: {relationToAdd.FriendId} not found.");
					}
				}
				else
				{
					return BadRequest($"{userId} is not valid Guid.");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during adding the user relation. User id: {user}, friend id {FriendId}", userId, relationToAdd.FriendId);
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}
		[HttpDelete("{friendId}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult> DeleteRelation(string userId, string friendId)
		{
			try
			{
				if (Guid.TryParse(userId, out Guid gUserId) && Guid.TryParse(friendId, out Guid gFriendId))
				{
					if (await _userRepository.CheckIfUserExists(gUserId) && await _userRepository.CheckIfUserExists(gFriendId))
					{
						var relationFromRepo = _relationRepository.GetFriendRelation(gUserId, gFriendId);
						if (relationFromRepo is null)
						{
							return NotFound($"Users: {userId} and {friendId} relation not found");
						}
						await _relationRepository.DeleteRelationAsync(relationFromRepo);
						return NoContent();
					}
					else
					{
						return NotFound($"User: {userId} or friend: {gFriendId} not found.");
					}
				}
				else
				{
					return BadRequest($"{userId} or {friendId} is not valid Guid.");
				}
				}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during removing the friends relation. Users id: {userId} and {friendId}", userId, friendId);
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}
	}
}
