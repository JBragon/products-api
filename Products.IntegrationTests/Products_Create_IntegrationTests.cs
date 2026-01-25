using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Products.IntegrationTests
{
    public class Products_Create_IntegrationTests : IntegrationTestBase
    {
        public Products_Create_IntegrationTests(ApiFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Post_ShouldReturn400_WhenIdempotencyKeyIsMissing()
        {
            // Act
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/products")
            {
                Content = CreateJsonContent(new { title = "Test" })
            };
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Post_ShouldReturn422_WhenPayloadIsInvalid()
        {
            // Arrange
            var invalidPayload = new
            {
                title = "T", // Too short (FluetValidation)
                price = 100, // Valid (DataAnnotations)
                currency = "USD",
                model = "Test Model Invalid",
                brand = "Test Brand",
                condition = "New",
                stock = 10
            };

            // Act
            var response = await _client.SendAsync(CreatePostRequest("/api/products", invalidPayload, Guid.NewGuid().ToString()));

            // Assert
            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
            var problemDetails = await DeserializeResponse<ValidationProblemDetails>(response);
            Assert.NotNull(problemDetails);
            Assert.NotEmpty(problemDetails.Errors);
        }

        [Fact]
        public async Task Post_ShouldReturn201_WhenPayloadIsValid()
        {
            // Arrange
            var payload = new
            {
                title = "Integration Test Product",
                price = 100.50,
                currency = "USD",
                stock = 50,
                brand = "Test Brand",
                model = "Test Model",
                condition = "New"
            };

            // Act
            var response = await _client.SendAsync(CreatePostRequest("/api/products", payload, Guid.NewGuid().ToString()));

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(response.Headers.Location);

            // Verify existence
            var getResponse = await _client.GetAsync(response.Headers.Location);
            getResponse.EnsureSuccessStatusCode();
            var createdProduct = await DeserializeResponse<ProductDetailResponse>(getResponse);
            Assert.NotNull(createdProduct);
            Assert.Equal(payload.title, createdProduct.Title);
        }

        [Fact]
        public async Task Post_ShouldBeIdempotent_WhenSameKeyIsUsed()
        {
            // Arrange
            var payload = new
            {
                title = "Idempotent Product",
                price = 200,
                currency = "EUR",
                model = "Test Model 2",
                brand = "Test Brand",
                condition = "New",
                stock = 10
            };
            var key = Guid.NewGuid().ToString();

            // Act 1
            var response1 = await _client.SendAsync(CreatePostRequest("/api/products", payload, key));
            response1.EnsureSuccessStatusCode();
            var location1 = response1.Headers.Location;
            // Extract ID from Location (assuming /api/products/{id})
            var id1 = location1?.LocalPath.Split('/').Last();

            // Act 2 (Retry)
            var response2 = await _client.SendAsync(CreatePostRequest("/api/products", payload, key));
            response2.EnsureSuccessStatusCode();
            var location2 = response2.Headers.Location;
            var id2 = location2?.LocalPath.Split('/').Last();

            // Assert
            Assert.Equal(HttpStatusCode.Created, response1.StatusCode);
            Assert.Equal(HttpStatusCode.Created, response2.StatusCode);
            Assert.Equal(id1, id2);
        }
    }

    public class ValidationProblemDetails
    {
        public string Title { get; set; } = string.Empty;
        public int Status { get; set; }
        public Dictionary<string, string[]> Errors { get; set; } = new();
    }
}
