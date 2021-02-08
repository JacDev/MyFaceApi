using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

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
		public static IEnumerable<ApiResource> GetApis() =>
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
		public static IEnumerable<Client> GetClients(IConfigurationSection configurationSection)
		{
			return new List<Client>
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
					RedirectUris =           { $"{configurationSection.GetValue<string>("RedirectUris")}signin-callback", $"{configurationSection.GetValue<string>("RedirectUris")}assets/silent-callback.html" },
					PostLogoutRedirectUris = { configurationSection.GetValue<string>("PostLogoutRedirectUris") },
					AllowedCorsOrigins =     { configurationSection.GetValue<string>("ClientId") },
					AccessTokenLifetime = 600,
					AllowedScopes =
					{
						IdentityServerConstants.StandardScopes.OpenId,
						IdentityServerConstants.StandardScopes.Profile,
						"MyFaceApiV2",
						"userInfo",
						"UserInfo2"
					},

				},
			};
		}
	};
}
