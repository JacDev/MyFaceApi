using System;

namespace MyFaceApi.Api.Application.DtoModels.Message
{
	public class MessageDto
	{
		public Guid Id { get; set; }
		public Guid ToWho { get; set; }
		public string Text { get; set; }
		public DateTime When { get; set; }
		public Guid FromWho { get; set; }
	}
}
