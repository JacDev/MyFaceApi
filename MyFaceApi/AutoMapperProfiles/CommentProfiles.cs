using AutoMapper;
using MyFaceApi.Api.DataAccess.Entities;
using MyFaceApi.Api.Models.CommentModels;

namespace MyFaceApi.AutoMapperProfiles
{
	public class CommentProfiles : Profile
	{
		public CommentProfiles()
		{
			CreateMap<CommentToAdd, PostComment>();
			CreateMap<PostComment, CommentToAdd>();
			CreateMap<PostComment, CommentToUpdate>();
			CreateMap<CommentToUpdate, PostComment>();
		}
	}
}
