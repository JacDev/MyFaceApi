using MyFaceApi.Api.DataAccess.ModelsBasicInfo;
using System;

namespace MyFaceApi.Api.DboModels
{
	public class PostDbo : BasicPostData
	{
		public Guid Id { get; set; }
		public DateTime WhenAdded { get; set; }
		public Guid UserId { get; set; }
		public int PostCommentsCounter { get; set; }
		public int PostReactionsCounter { get; set; }
	}
}
