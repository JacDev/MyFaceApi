using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MyFaceApi.Entities
{
	public class User : IdentityUser<Guid>
	{
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [AllowNull]
        public string ProfileImagePath { get; set; } = null;
        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<FriendRelation> Relations { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public DateTime DateOfBirht { get; set; }

        public User()
        {
            Relations = new List<FriendRelation>();
            Posts = new List<Post>();
            Notifications = new List<Notification>();
        }
    }
}
