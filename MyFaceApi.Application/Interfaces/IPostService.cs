using MyFaceApi.Api.Application.DtoModels.Post;
using MyFaceApi.Api.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyFaceApi.Api.Application.Interfaces
{
	public interface IPostService
	{
		Task<PostDto> AddPostAsync(Guid userId, PostToAddDto post);
		bool CheckIfPostExists(Guid postId);
		Task DeletePostAsync(Guid postId);
		PostDto GetPost(Guid postId);
		List<PostDto> GetUserPosts(Guid userId);
		List<PostDto> GetLatestFriendsPosts(Guid userId, List<Guid> userFriends);
		Task UpdatePostAsync(Post post)
	}
}
