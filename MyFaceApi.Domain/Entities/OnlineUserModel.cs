namespace MyFaceApi.Api.Domain.Entities
{
	public class OnlineUserModel
	{
		public string Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string ConnectionId { get; set; } = string.Empty;
	}
}
