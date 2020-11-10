using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyFaceApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFaceApi.Data
{
	public interface IAppDbContext
	{
		DbSet<User> Users { get; set; }
		DbSet<FriendRelation> Relations { get; set; }
		DbSet<Post> Posts { get; set; }
		DbSet<Notification> Notifications { get; set; }
		Task<int> SaveAsync();
	}
}
