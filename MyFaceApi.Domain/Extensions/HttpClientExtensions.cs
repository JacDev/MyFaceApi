using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyFaceApi.Api.Domain.Extensions
{
	public static class HttpClientExtensions
	{
		public static Task<HttpResponseMessage> GetFromApiAsync(this HttpClient httpClient, string url)
		{
			return httpClient.GetAsync(url);
		}
		public static Task<HttpResponseMessage> PatchToApiAsJsonAsync<T>(this HttpClient httpClient, string url, T data)
		{
			StringContent content = AddContent(data);
			return httpClient.PatchAsync(url, content);
		}
		public static async Task<T> ReadContentAs<T>(this HttpResponseMessage response)
		{
			if (!response.IsSuccessStatusCode)
			{
				throw new ApplicationException($"Something went wrong calling the API. HTTP code: {response.StatusCode}");
			}

			string dataAsString = await response.Content.ReadAsStringAsync();
			var responseData = JsonSerializer.Deserialize<T>(dataAsString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
			return responseData;
		}
		private static StringContent AddContent<T>(T data)
		{

			string dataAsString = JsonSerializer.Serialize(data);
			StringContent content = new StringContent(dataAsString);
			content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
			return content;
		}
		public static Task<HttpResponseMessage> PostToApiAsJsonAsync<T>(this HttpClient httpClient, string url, T data)
		{
			StringContent content = AddContent(data);
			return httpClient.PostAsync(url, content);
		}
	}
}
