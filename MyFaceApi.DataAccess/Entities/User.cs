﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MyFaceApi.DataAccess.Entities
{
	public class User : IdentityUser<Guid>
	{
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [AllowNull]
        public string ProfileImagePath { get; set; } = null;
        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<FriendsRelation> Relations { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public DateTime DateOfBirht { get; set; }

        public User()
        {
            Relations = new List<FriendsRelation>();
            Posts = new List<Post>();
            Notifications = new List<Notification>();
        }
    }
}
