using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;

namespace MyFaceApi.Api.Servieces
{
	public class IdentityServerHttpService : IIdentityServerHttpService
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
