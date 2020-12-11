using System;
using System.Collections.Generic;

namespace MyFaceApi.Api.Domain.Entities
{
	public class Conversation
	{
		public Guid Id { get; set; }
		public Guid FirstUser { get; set; }
		public Guid SecondUser { get; set; }
		public ICollection<Message> Messages { get; set; }
	}
}