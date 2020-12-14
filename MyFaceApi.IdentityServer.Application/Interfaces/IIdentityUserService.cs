using MyFaceApi.Api.IdentityServer.Application.DtoModels.User;
using Pagination.Helpers;
using System;
using System.Collections.Generic;

namespace MyFaceApi.IdentityServer.Application.Interfaces
{
	public interface IIdentityUserService
	{
		IdentityUserDto GetUser(Guid userId);
		List<IdentityUserDto> GetUsers(string[] ids);
		PagedList<IdentityUserDto> GetUsers(string searchString, PaginationParams paginationParams);
	}
}