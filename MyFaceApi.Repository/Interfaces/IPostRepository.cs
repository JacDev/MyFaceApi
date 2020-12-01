using MyFaceApi.Api.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyFaceApi.Api.Repository.Interfaces
{
	public interface IPostRepository
	{
		Task<Post> AddPostAsync(Post post);
		bool CheckIfPostExists(Guid postId);
		Task DeletePostAsync(Post postId);
		Post GetPost(Guid postId);
		List<Post> GetUserPosts(Guid userId);
		Task UpdatePostAsync(Post post);
	}
}
