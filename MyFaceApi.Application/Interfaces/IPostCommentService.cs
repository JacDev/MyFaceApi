using MyFaceApi.Api.Application.DtoModels.Comment;
using MyFaceApi.Api.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyFaceApi.Api.Application.Interfaces
{
	public interface IPostCommentService
	{
		Task<CommentDto> AddCommentAsync(Guid postId, CommentToAddDto postComment);
		Task DeleteCommentAsync(Guid comment);
		List<CommentDto> GetPostComments(Guid postId);
		CommentDto GetComment(Guid commentId);
		Task UpdateComment(PostComment postComment);
	}
}
