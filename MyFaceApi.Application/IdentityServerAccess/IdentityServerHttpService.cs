using System;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using MyFaceApi.Api.Domain.ExternalApiInterfaces;

namespace MyFaceApi.Api.Application.IdentityServerAccess
{
	public class IdentityServerHttpService : IHttpService
	{
		public HttpClient Client { get; }
		public IdentityServerHttpService(IHttpClientFactory clientFactory, IConfiguration configuration)
		{
			Client = clientFactory.CreateClient();
			IConfigurationSection identityServerConf = configuration.GetSection("IdentityServerConfiguration");
			Client.BaseAddress = new Uri(identityServerConf.GetValue<string>("IdentityServerUri"));
		}
	}
}
