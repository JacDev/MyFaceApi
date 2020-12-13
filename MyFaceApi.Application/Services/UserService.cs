﻿using MyFaceApi.Api.Application.DtoModels.User;
using MyFaceApi.Api.Domain.ExternalApiInterfaces;
using System;
using System.Threading.Tasks;
using MyFaceApi.Api.Domain.Extensions;
using System.Collections.Generic;
using MyFaceApi.Api.Application.Interfaces;

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
			var response = await _identityServerHttpService.Client.GetAsync($"/users/{userId}");
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
			var response = await _identityServerHttpService.Client.GetAsync($"/users/getall?ids={query}");
			return await response.ReadContentAs<List<UserDto>>();
		}
	}
}