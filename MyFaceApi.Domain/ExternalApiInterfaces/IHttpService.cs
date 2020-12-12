using System.Net.Http;

namespace MyFaceApi.Api.Domain.ExternalApiInterfaces
{
	public interface IHttpService
	{
		HttpClient Client { get; }
	}
}
