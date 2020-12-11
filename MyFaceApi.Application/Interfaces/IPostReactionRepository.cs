using MyFaceApi.Api.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyFaceApi.Api.Repository.Interfaces
{
	public interface IPostReactionRepository
	{
		Task<PostReaction> AddPostReactionAsync(PostReaction postReaction);
		Task DeletePostReactionAsync(PostReaction postReaction);
		List<PostReaction> GetPostReactions(Guid postId);
		PostReaction GetPostReaction(Guid reactionId);
		PostReaction GetPostReaction(Guid fromWho, Guid postId);
		Task UpdatePostReactionAsync(PostReaction postReaction);
	}
}
