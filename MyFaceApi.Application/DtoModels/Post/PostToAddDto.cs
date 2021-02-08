using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace MyFaceApi.Api.Application.DtoModels.Post
{
	public class PostToAddDto
	{
		public string Text { get; set; }
		public DateTime WhenAdded { get; set; }
		public IFormFile Picture { get; set; }
	}
}
