using MyFaceApi.Api.DataAccess.ModelsBasicInfo;

namespace MyFaceApi.Api.Models.UserModels
{
	public class UserWithCountersDbo : BasicUserData
	{
		public int NewNotificationsCounter { get; set; }
		public int TotalFriendsCounter { get; set; }
		public int TotalPostCounter { get; set; }
	}
}
