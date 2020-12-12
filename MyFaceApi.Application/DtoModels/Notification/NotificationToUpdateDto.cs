using System.ComponentModel.DataAnnotations;

namespace MyFaceApi.Api.Application.DtoModels.Notification
{
	public class NotificationToUpdateDto
	{
		[Required]
		public bool HasSeen { get; set; }
	}
}
