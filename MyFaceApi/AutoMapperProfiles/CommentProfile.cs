using AutoMapper;
using MyFaceApi.Entities;
using MyFaceApi.Models.CommentModels;

namespace MyFaceApi.AutoMapperProfiles
{
	public class CommentProfile : Profile
	{
		public CommentProfile()
		{
			CreateMap<CommentToAdd, PostComment>();
			CreateMap<PostComment, CommentToAdd>();
			CreateMap<PostComment, CommentToUpdate>();
			CreateMap<CommentToUpdate, PostComment>();
		}
	}
}
