using System.Text;
using System.Text.Json;
using Xunit;

namespace Products.IntegrationTests
{
    public abstract class IntegrationTestBase : IClassFixture<ApiFactory>
    {
        protected readonly HttpClient _client;
        protected readonly JsonSerializerOptions _jsonOptions;
        protected readonly ApiFactory _factory;

        protected IntegrationTestBase(ApiFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };
        }

        protected StringContent CreateJsonContent(object data)
        {
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        protected async Task<T?> DeserializeResponse<T>(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(content))
            {
                return default;
            }
            return JsonSerializer.Deserialize<T>(content, _jsonOptions);
        }

        protected HttpRequestMessage CreatePostRequest(string url, object body, string? idempotencyKey = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = CreateJsonContent(body)
            };

            if (idempotencyKey != null)
            {
                request.Headers.Add("Idempotency-Key", idempotencyKey);
            }

            return request;
        }
    }
}
