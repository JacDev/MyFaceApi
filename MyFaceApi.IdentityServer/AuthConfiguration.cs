using IdentityServer4;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFaceApi.IdentityServer
{
	public static class AuthConfiguration
	{
		public static IEnumerable<IdentityResource> GetIdentityResources() =>
			new List<IdentityResource>
			{
				new IdentityResources.OpenId(),
				new IdentityResources.Profile(),
				new IdentityResource
				{
					Name = "userInfo",
					UserClaims =
					{
						"FirstName",
						"LastName",
						"ProfileImagePath",
						"DateOfBirht"
					}
				}
			};
		public static IEnumerable<ApiScope> GetApiScopes()
		{
			return new List<ApiScope>
			{
					new ApiScope(name: "MyFaceApiV2",   displayName: "Read your data."),
					new ApiScope(name: "UserInfo2", displayName: "IdentityUserInfo")
			};
		}
		public static IEnumerable<ApiResource> GetApis() => //jakie sa api i co mogą dostać
			new List<ApiResource>
			{
				new ApiResource()
				{
					Name = "MyFaceApiV2",
					Scopes = { "MyFaceApiV2" }
				},
				new ApiResource()
				{
					Name = "UserInfo2",

					UserClaims =
					{
						"FirstName",
						"LastName",
						"ProfileImagePath",
						"DateOfBirht"
					}
				}
			};
		public static IEnumerable<Client> GetClients() =>
			new List<Client>
			{
				new Client
				{
					ClientId = "MyFaceAngularClient",
					ClientName = "MyFace",
					RequireClientSecret = false,
					AllowedGrantTypes = GrantTypes.Code,
					RequirePkce = true,
					AllowAccessTokensViaBrowser = true,
					RequireConsent = false,


					RedirectUris =           { "http://localhost:4200/signin-callback",  },
					PostLogoutRedirectUris = { "http://localhost:4200/signout-callback" },
					AllowedCorsOrigins =     { "http://localhost:4200" },

					AllowedScopes =
					{
						IdentityServerConstants.StandardScopes.OpenId,
						IdentityServerConstants.StandardScopes.Profile,
						"MyFaceApiV2",
						"userInfo",
						"UserInfo2"
					},

				},
				//new Client {
				//	ClientId = "MyFaceJsClient",

				//	AllowedGrantTypes = GrantTypes.Code,
				//	RequirePkce = true,
				//	RequireClientSecret = false,

				//	RedirectUris = { "https://localhost:44393/message/jslogin" },
				//	PostLogoutRedirectUris = { "https://localhost:44393/Test/Index" },
				//	AllowedCorsOrigins = { "https://localhost:44393" },

				//	AllowedScopes = {
				//		IdentityServerConstants.StandardScopes.OpenId,
				//		IdentityServerConstants.StandardScopes.Profile,
				//		"MyFaceApi",
				//		"userinfo"
				//	},

				//	AccessTokenLifetime = 1,

				//	AllowAccessTokensViaBrowser = true,
				//	RequireConsent = false,
				//}
			};
	};
}
