using AutoMapper;
using MyFaceApi.DataAccess.Entities;
using MyFaceApi.Models.CommentModels;

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
