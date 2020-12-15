using Microsoft.AspNetCore.JsonPatch;
using MyFaceApi.Api.Application.DtoModels.PostComment;
using Pagination.Helpers;
using System;
using System.Threading.Tasks;

namespace MyFaceApi.Api.Application.Interfaces
{
	public interface IPostCommentService
	{
		Task<PostCommentDto> AddCommentAsync(Guid postId, PostCommentToAddDto postComment);
		Task DeleteCommentAsync(Guid comment);
		PagedList<PostCommentDto> GetPostComments(Guid postId, PaginationParams paginationParams);
		PostCommentDto GetComment(Guid commentId);
		Task<bool> TryUpdatePostCommentAsync(Guid commentId, JsonPatchDocument<PostCommentToUpdateDto> patchDocument);
	}
}
