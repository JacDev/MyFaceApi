using System;
using System.ComponentModel.DataAnnotations;

namespace MyFaceApi.Api.Application.DtoModels.Message
{
	public class MessageToAddDto
	{
		[Required]
		public string Text { get; set; }
		[Required]
		public DateTime When { get; set; }
		[Required]
		public Guid FromWho { get; set; }
	}
}