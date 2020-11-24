using MyFaceApi.DataAccess.ModelsBasicInfo;
using System;

namespace MyFaceApi.Models.PostModels
{
	public class PostToAdd : BasicPostData
	{
		public DateTime WhenAdded { get; set; }
	}
}
