using AutoMapper;
using MyFaceApi.Api.IdentityServer.Application.DtoModels.User;
using MyFaceApi.IdentityServer.Application.Interfaces;
using MyFaceApi.IdentityServer.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using MyFaceApi.IdentityServer.Infrastructure.Database;
using Pagination.Helpers;

namespace MyFaceApi.IdentityServer.Application.Services
{
	public class IdentityUserService : IIdentityUserService
	{
		private readonly IdentityServerDbContext _dbContext;
		private readonly IMapper _mapper;
		public IdentityUserService(IdentityServerDbContext dbContext,
			IMapper mapper)
		{
			_dbContext = dbContext;
			_mapper = mapper;
		}
		public IdentityUserDto GetUser(Guid userId)
		{
			ApplicationUser user = _dbContext.Users.FirstOrDefault(x => x.Id == userId);
			return _mapper.Map<IdentityUserDto>(user);
		}
		public List<IdentityUserDto> GetUsers(string[] ids)
		{
			List<IdentityUserDto> usersToReturn = new List<IdentityUserDto>();
			foreach (var id in ids)
			{
				if (Guid.TryParse(id, out Guid gId))
				{
					var userToAdd = _dbContext.Users.FirstOrDefault(x => x.Id == gId);
					usersToReturn.Add(_mapper.Map<IdentityUserDto>(userToAdd));
				}
				else
				{
					throw new ArgumentException();
				}
			}
			return usersToReturn;
		}
		public PagedList<IdentityUserDto> GetUsers(string searchString, PaginationParams paginationParams)
		{
			List<ApplicationUser> foundUsers = _dbContext.Users
					.Where(s => s.LastName.Contains(searchString) || s.FirstName.Contains(searchString)).ToList();
			List<IdentityUserDto> usersToReturn = _mapper.Map<List<IdentityUserDto>>(foundUsers);

			return PagedList<IdentityUserDto>.Create(usersToReturn,
							paginationParams.PageNumber,
							paginationParams.PageSize,
							(paginationParams.PageNumber - 1) * paginationParams.PageSize + paginationParams.Skip);
		}
	}
}
