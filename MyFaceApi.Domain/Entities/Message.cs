using System;

namespace MyFaceApi.Api.Domain.Entities
{
	public class Message
	{
		public Guid Id { get; set; }
		public Guid ToWho { get; set; }
		public string Text { get; set; }
		public DateTime When { get; set; }
		public Guid FromWho { get; set; }
	}
}
