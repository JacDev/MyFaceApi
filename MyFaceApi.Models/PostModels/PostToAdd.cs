using MyFaceApi.Api.DataAccess.ModelsBasicInfo;
using System;

namespace MyFaceApi.Api.Models.PostModels
{
	public class PostToAdd : BasicPostData
	{
		public DateTime WhenAdded { get; set; }
	}
}
