using MyFaceApi.Api.DataAccess.ModelsBasicInfo;
using Microsoft.AspNetCore.Http;

namespace MyFaceApi.Api.Models.PostModels
{
	public class PostToAdd : BasicPostData
	{
		public IFormFile Picture { get; set; } = null;
	}
}
