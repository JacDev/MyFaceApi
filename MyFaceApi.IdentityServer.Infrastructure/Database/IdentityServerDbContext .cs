using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyFaceApi.IdentityServer.Domain.Entities;
using System;

namespace MyFaceApi.IdentityServer.Infrastructure.Database
{
	public class IdentityServerDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
	{
		public IdentityServerDbContext(DbContextOptions<IdentityServerDbContext> options) : base(options)
		{
		}
	}
}