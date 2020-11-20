using MyFaceApi.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyFaceApi.Repository.Interfaceses
{
	public interface IPostReactionRepository
	{
		Task<PostReaction> AddPostReactionAsync(PostReaction postReaction);
		Task DeletePostReactionAsync(PostReaction postReaction);
		List<PostReaction> GetPostReactions(Guid postId);
		PostReaction GetPostReaction(Guid reactionId);
		Task UpdatePostReactionAsync(PostReaction postReaction);
	}
}
