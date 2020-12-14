using MyFaceApi.Api.Application.DtoModels.User;
using MyFaceApi.Api.Domain.ExternalApiInterfaces;
using System;
using System.Threading.Tasks;
using MyFaceApi.Api.Domain.Extensions;
using System.Collections.Generic;
using MyFaceApi.Api.Application.Interfaces;
using System.Net.Http;
using Pagination.Helpers;
using Pagination.DtoModels;

namespace MyFaceApi.Api.Application.Services
{
	public class UserService : IUserService
	{
		private readonly IHttpService _identityServerHttpService;
		public UserService(IHttpService identityServerHttpService)
		{
			_identityServerHttpService = identityServerHttpService;
		}
		public async Task<UserDto> GetUserAsync(Guid userId)
		{
			HttpResponseMessage response = await _identityServerHttpService.Client.GetAsync($"/users/{userId}");
			return await response.ReadContentAs<UserDto>();
		}

		public async Task<bool> CheckIfUserExists(Guid userId)
		{
			return await GetUserAsync(userId) != null;
		}
		public async Task<List<UserDto>> GetUsersAsync(IEnumerable<Guid> usersId)
		{
			string query = "";
			foreach (var id in usersId)
			{
				query += id.ToString() + ",";
			}
			query = query.Remove(query.LastIndexOf(","));
			HttpResponseMessage response = await _identityServerHttpService.Client.GetAsync($"/users/getall?ids={query}");
			return await response.ReadContentAs<List<UserDto>>();
		}
		public async Task<PagedList<UserDto>> GetUsersAsync(string searchString, PaginationParams paginationParams)
		{
			HttpResponseMessage response = await _identityServerHttpService.Client
			.GetAsync($"/users/with?searchString={searchString}&pageNumber={paginationParams.PageNumber}&pageSize={paginationParams.PageSize}&skip={paginationParams.Skip}");
			var content =  await response.ReadContentAs<CollectionWithPaginationData<UserDto>>();
			content.Collection.AddMetadataParams(content.PaginationMetadata);
			return content.Collection;
		}
	}
}