using Microsoft.AspNetCore.JsonPatch;
using MyFaceApi.Api.Application.DtoModels.PostReaction;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyFaceApi.Api.Application.Interfaces
{
	public interface IPostReactionService
	{
		Task<PostReactionDto> AddPostReactionAsync(Guid postId, PostReactionToAddDto postReaction);
		Task DeletePostReactionAsync(Guid postReactionId);
		Task DeletePostReactionAsync(Guid userId, Guid postId);
		List<PostReactionDto> GetPostReactions(Guid postId);
		PostReactionDto GetPostReaction(Guid reactionId);
		PostReactionDto GetPostReaction(Guid fromWho, Guid postId);
		Task<bool> TryUpdatePostReactionAsync(Guid reactionId, JsonPatchDocument<PostReactionToUpdate> patchDocument);
	}
}
