using System;
using System.Collections.Generic;
using System.Text;

namespace MyFaceApi.DataAccess.ModelsBasicInfo
{
	public class BasicMessageData
	{
		public string Text { get; set; }
		public DateTime When { get; set; }
		public Guid FromWho { get; set; }
	}
}
