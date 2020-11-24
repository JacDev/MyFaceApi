using IdentityServer4;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFaceApi.IdentityServer
{
	public static class Configuration
	{
		public static IEnumerable<IdentityResource> GetIdentityResources() =>
			new List<IdentityResource>
			{
				new IdentityResources.OpenId(),
				new IdentityResources.Profile(),
				new IdentityResource
				{
					Name = "userinfo",
					UserClaims =
					{
						"FirstName",
						"LastName",
						"ProfileImagePath",
						"DateOfBirht"
					}
				}
			};
		public static IEnumerable<ApiResource> GetApis() => //jakie sa api i co mogą dostać
			new List<ApiResource>
			{
				new ApiResource("MyFaceApiV2")
			};
		public static IEnumerable<Client> GetClients() =>
			new List<Client>
			{
				new Client {
					ClientId = "MyFaceJsClient",

					AllowedGrantTypes = GrantTypes.Code,
					RequirePkce = true,
					RequireClientSecret = false,

					RedirectUris = { "https://localhost:44393/message/jslogin" },
					PostLogoutRedirectUris = { "https://localhost:44393/Test/Index" },
					AllowedCorsOrigins = { "https://localhost:44393" },

					AllowedScopes = {
						IdentityServerConstants.StandardScopes.OpenId,
						IdentityServerConstants.StandardScopes.Profile,
						"MyFaceApi",
						"userinfo"
					},

					AccessTokenLifetime = 1,

					AllowAccessTokensViaBrowser = true,
					RequireConsent = false,
				}
			};
	};
}
