using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyFaceApi.IdentityServer.DataAccess.Entities;
using System;

namespace MyFaceApi.IdentityServer.DataAccess.Data
{
	public class IdentityServerDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
	{
		public IdentityServerDbContext(DbContextOptions<IdentityServerDbContext> options) : base(options)
		{
		}
	}
}
