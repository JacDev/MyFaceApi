using MyFaceApi.Api.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyFaceApi.Api.Repository.Interfaces
{
	public interface IPostCommentRepository
	{
		Task<PostComment> AddCommentAsync(PostComment postComment);
		Task DeleteCommentAsync(PostComment comment);
		List<PostComment> GetComments(Guid postId);
		PostComment GetComment(Guid commentId);
		Task UpdateComment(PostComment postComment);
	}
}
