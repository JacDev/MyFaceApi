using AutoMapper;
using MyFaceApi.Api.Application.DtoModels.PostComment;
using MyFaceApi.Api.Domain.Entities;

namespace MyFaceApi.Api.Application.AutoMapperProfiles
{
	public class CommentProfiles : Profile
	{
		public CommentProfiles()
		{
			CreateMap<PostCommentToAddDto, PostComment>();
			CreateMap<PostComment, PostCommentDto>();
			CreateMap<PostComment, PostCommentToUpdateDto>();
			CreateMap<PostCommentToUpdateDto, PostComment>();
		}
	}
}
