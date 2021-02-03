using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MyFaceApi.Api.Application.FileManagerInterfaces
{
	public interface IImageManager
	{
		string ImagePath { get; set; }

		Task<Tuple<string, string>> SaveImage(IFormFile image);
		FileStream ImageStream(string imageName);
		string AddProfileImage(string imageName, int width, int height);
	}
}