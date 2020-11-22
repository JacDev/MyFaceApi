using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyFaceApi.DataAccess.Entities;
using MyFaceApi.Models.FriendsRelationModels;
using MyFaceApi.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyFaceApi.Controllers
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
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public ActionResult<List<FriendsRelation>> GetUserRelations(string userId)
		{
			try
			{
				if (Guid.TryParse(userId, out Guid gUserId))
				{
					if (_userRepository.CheckIfUserExists(gUserId))
					{
						var userRelations = _relationRepository.GetUserRelationships(gUserId);
						return Ok(userRelations);
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
					if (_userRepository.CheckIfUserExists(gUserId) && _userRepository.CheckIfUserExists(relationToAdd.FriendId))
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
					if (_userRepository.CheckIfUserExists(gUserId) && _userRepository.CheckIfUserExists(gFriendId))
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
