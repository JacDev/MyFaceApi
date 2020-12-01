using System.Net.Http;

namespace MyFaceApi.Api.Servieces
{
	public interface IIdentityServerHttpService
	{
		HttpClient Client { get; }
	}
}
