using Microsoft.AspNetCore.JsonPatch;
using MyFaceApi.Api.Application.DtoModels.Comment;
using MyFaceApi.Api.Application.Helpers;
using System;
using System.Threading.Tasks;

namespace MyFaceApi.Api.Application.Interfaces
{
	public interface IPostCommentService
	{
		Task<CommentDto> AddCommentAsync(Guid postId, CommentToAddDto postComment);
		Task DeleteCommentAsync(Guid comment);
		PagedList<CommentDto> GetPostComments(Guid postId, PaginationParams paginationParams);
		CommentDto GetComment(Guid commentId);
		Task<bool> TryUpdatePostCommentAsync(Guid commentId, JsonPatchDocument<CommentToUpdateDto> patchDocument);
	}
}
