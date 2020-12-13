using MyFaceApi.Api.IdentityServer.Application.DtoModels.User;
using System;
using System.Collections.Generic;

namespace MyFaceApi.IdentityServer.Application.Interfaces
{
	public interface IIdentityUserService
	{
		IdentityUserDto GetUser(Guid userId);
		List<IdentityUserDto> GetUsers(string[] ids);
	}
}