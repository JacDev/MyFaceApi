using AutoMapper;
using MyFaceApi.Api.Application.DtoModels.Comment;
using MyFaceApi.Api.Domain.Entities;

namespace MyFaceApi.AutoMapperProfiles
{
	public class CommentProfiles : Profile
	{
		public CommentProfiles()
		{
			CreateMap<CommentToAddDto, PostComment>();
			CreateMap<PostComment, CommentDto>();
		}
	}
}
