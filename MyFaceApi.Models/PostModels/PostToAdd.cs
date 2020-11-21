using MyFaceApi.DataAccess.ModelsBasicInfo;
using System;

namespace MyFaceApi.Models.PostModels
{
	public class PostToAdd : BasicPostInfo
	{
		public DateTime WhenAdded { get; set; }
	}
}
