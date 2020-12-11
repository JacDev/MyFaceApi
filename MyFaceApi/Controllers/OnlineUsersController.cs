using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyFaceApi.Api.DataAccess.ModelsBasicInfo;
using MyFaceApi.Api.DboModels;
using MyFaceApi.Api.Extensions;
using MyFaceApi.Api.Helpers;
using MyFaceApi.Api.Repository.Helpers;
using MyFaceApi.Api.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFaceApi.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class OnlineUsersController : ControllerBase
	{
		private readonly IFriendsRelationRepository _friendsRelationRepository;
		private readonly IOnlineUsersRepository _onlineUsersRepository;
		private readonly ILogger<OnlineUsersController> _logger;
		private readonly IMapper _mapper;
		private readonly IUserRepository _userRepository;
		public OnlineUsersController(IOnlineUsersRepository onlineUsersRepository,
			IFriendsRelationRepository friendsRelationRepository,
			ILogger<OnlineUsersController> logger,
			IMapper mapper,
			IUserRepository userRepository)
		{
			_onlineUsersRepository = onlineUsersRepository;
			_friendsRelationRepository = friendsRelationRepository;
			_logger = logger;
			_mapper = mapper;
			_userRepository = userRepository;
		}
		[HttpGet(Name = "GetOnlineFriends")]
		[AllowAnonymous]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<CollectionWithPaginationData<BasicUserData>>> GetOnlineFriends(string userId, [FromQuery] PaginationParams paginationParams)
		{
			try
			{
				if (Guid.TryParse(userId, out Guid gUserId))
				{
					paginationParams.Skip = (paginationParams.PageNumber - 1) * paginationParams.PageSize;
					if (await _userRepository.CheckIfUserExists(gUserId))
					{

						List<Guid> onlineFriendsIds = await GetOnlineFriendsIds(gUserId);

						PagedList<BasicUserData> onlineFriendsToReturn = await GetOnlineFriendsFromRepo(onlineFriendsIds, paginationParams); 

						PaginationMetadata pagination = PaginationHelper.CreatePaginationMetadata(onlineFriendsToReturn);

						return Ok(new CollectionWithPaginationData<BasicUserData> { PaginationMetadata = pagination, Collection = onlineFriendsToReturn });
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
		private async Task<List<Guid>> GetOnlineFriendsIds(Guid userId)
		{
			List<Guid> onlineUsersIds = await _onlineUsersRepository.GetOnlineUsersAsync();
			List<Guid> userFriendsIds = await _friendsRelationRepository.GetUserFriendsAsync(userId);

			return onlineUsersIds.Intersect(userFriendsIds).ToList();
		}
		private async Task<PagedList<BasicUserData>> GetOnlineFriendsFromRepo(List<Guid> onlineFriendsIds, PaginationParams paginationParams)
		{
			PagedList<Guid> friendsToReturnIds = PagedList<Guid>.Create(onlineFriendsIds,
							paginationParams.PageNumber,
							paginationParams.PageSize,
							(paginationParams.PageNumber - 1) * paginationParams.PageSize + paginationParams.Skip);

			List<BasicUserData> onlineFriendsFromRepo = await _userRepository.GetUsersAsync(friendsToReturnIds);

			var friendsToReturn =  PagedList<BasicUserData>.Create(onlineFriendsFromRepo,
				paginationParams.PageNumber,
				paginationParams.PageSize,
				(paginationParams.PageNumber - 1) * paginationParams.PageSize + paginationParams.Skip);

			friendsToReturn.TotalCount = friendsToReturnIds.TotalCount;
			friendsToReturn.TotalPages = friendsToReturnIds.TotalPages;

			friendsToReturn.PreviousPageLink = friendsToReturn.HasPrevious ?
							this.CreateMessagesResourceUriWithPaginationParams(paginationParams, ResourceUriType.PreviousPage, "GetOnlineFriends") : null;

			friendsToReturn.NextPageLink = friendsToReturn.HasNext ?
				this.CreateMessagesResourceUriWithPaginationParams(paginationParams, ResourceUriType.NextPage, "GetOnlineFriends") : null;

			return friendsToReturn;
		}
	}
}
