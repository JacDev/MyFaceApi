using MyFaceApi.Api.Servieces;
using System.Threading.Tasks;
using MyFaceApi.Api.Services;
using MyFaceApi.Api.DataAccess.ModelsBasicInfo;

namespace MyFaceApi.Api.IdentityServerAccess
{
	public class UserIdentityServerAccess : IUserIdentityServerAccess
	{
		private readonly IIdentityServerHttpService _identityServerHttpService;

		public UserIdentityServerAccess(IIdentityServerHttpService identityServerHttpService)
		{
			_identityServerHttpService = identityServerHttpService;
		}
		public async Task<BasicUserData> GetUser(string userId)
		{
			var response = await _identityServerHttpService.Client.GetAsync($"/users/{userId}");
			return await response.ReadContentAs<BasicUserData>();
		}
	}
}
