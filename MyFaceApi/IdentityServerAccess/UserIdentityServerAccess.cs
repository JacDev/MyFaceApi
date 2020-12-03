using MyFaceApi.Api.Servieces;
using System.Threading.Tasks;
using MyFaceApi.Api.Services;
using MyFaceApi.Api.DataAccess.ModelsBasicInfo;
using MyFaceApi.Api.Repository.Interfaces;
using MyFaceApi.Api.DataAccess.Entities;
using System;
using System.Collections.Generic;

namespace MyFaceApi.Api.IdentityServerAccess
{
	public class UserIdentityServerAccess : IUserRepository
	{
		private readonly IIdentityServerHttpService _identityServerHttpService;

		public UserIdentityServerAccess(IIdentityServerHttpService identityServerHttpService)
		{
			_identityServerHttpService = identityServerHttpService;
		}

		public Task<User> AddUserAcync(User user)
		{
			throw new NotImplementedException();
		}

		public async Task<bool> CheckIfUserExists(Guid userId)
		{
			return await GetUserAsync(userId) != null;
		}

		public Task DeleteUserAsync(User user)
		{
			throw new NotImplementedException();
		}

		public async Task<BasicUserData> GetUserAsync(Guid userId)
		{
			var response = await _identityServerHttpService.Client.GetAsync($"/users/{userId}");
			return await response.ReadContentAs<BasicUserData>();
		}

		public async Task<bool> GetUserIfExists(Guid userId)
		{
			return await GetUserAsync(userId) != null;
		}
		public async Task<List<BasicUserData>> GetUsersAsync(IEnumerable<Guid> usersId)
		{
			string query = "";
			foreach(var id in usersId)
			{
				query += id.ToString() + ",";
			}
			query = query.Remove(query.LastIndexOf(","));
			var response = await _identityServerHttpService.Client.GetAsync($"/users/getall?ids={query}");
			return await response.ReadContentAs<List<BasicUserData>>();
		}

		public Task UpdateUserAsync(User user)
		{
			throw new NotImplementedException();
		}
	}
}
