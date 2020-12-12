using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using MyFaceApi.Api.Application.DtoModels.Post;
using MyFaceApi.Api.Application.Helpers;
using System;
using System.Threading.Tasks;

namespace MyFaceApi.Api.Application.Interfaces
{
	public interface IPostService
	{
		Task<PostDto> AddPostAsync(Guid userId, PostToAddDto post);
		bool CheckIfPostExists(Guid postId);
		Task DeletePostAsync(Guid postId);
		PostDto GetPost(Guid postId);
		PagedList<PostDto> GetUserPosts(Guid userId, PaginationParams paginationParams);
		PagedList<PostDto> GetLatestFriendsPosts(Guid userId, PaginationParams paginationParams);
		FileStreamResult StreamImage(string imageName);
		Task<bool> TryUpdatePostAsync(Guid postId, JsonPatchDocument<PostToUpdateDto> patchDocument);
	}
}
