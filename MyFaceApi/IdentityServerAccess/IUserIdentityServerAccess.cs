using MyFaceApi.Api.DataAccess.ModelsBasicInfo;
using MyFaceApi.Api.Models.UserModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyFaceApi.Api.IdentityServerAccess
{
	public interface IUserIdentityServerAccess
	{
		Task<BasicUserData> GetUser(string userId);
	}
}
