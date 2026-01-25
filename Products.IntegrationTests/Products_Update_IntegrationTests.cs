using System.Net;
using System.Text;
using System.Text.Json;
using Xunit;

namespace Products.IntegrationTests
{
    public class Products_Update_IntegrationTests : IntegrationTestBase
    {
        public Products_Update_IntegrationTests(ApiFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Put_ShouldReturn404_WhenProductDoesNotExist()
        {
            // Arrange
            var payload = new { 
                title = "Update NonExistent",
                brand = "Test Brand",
                model = "Test Model",
                condition = "New",
                price = 100,
                currency = "USD",
                stock = 10
            };
            var content = CreateJsonContent(payload);

            // Act
            var response = await _client.PutAsync($"/api/products/{Guid.NewGuid()}", content);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Put_ShouldReturn422_WhenBodyIsInvalid()
        {
            // Arrange
            // 1. Create a product first
            var createPayload = new
            {
                title = "Product To Fail Update",
                price = 100,
                currency = "USD",
                model = "Test Model Update 1",
                brand = "Test Brand",
                condition = "New",
                stock = 10
            };
            var createRes = await _client.SendAsync(CreatePostRequest("/api/products", createPayload, Guid.NewGuid().ToString()));
            createRes.EnsureSuccessStatusCode();
            var id = createRes.Headers.Location?.LocalPath.Split('/').Last();

            // 2. Try update with invalid data
            var invalidUpdate = new { 
                title = "T",
                brand = "Test Brand",
                model = "Test Model Invalid",
                condition = "New",
                price = 100, // Valid
                currency = "USD",
                stock = 10
            }; // too short
            var content = CreateJsonContent(invalidUpdate);

            // Act
            var response = await _client.PutAsync($"/api/products/{id}", content);

            // Assert
            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        }

        [Fact]
        public async Task Put_ShouldReturn204AndInvalidateCache_WhenUpdateIsValid()
        {
            // Arrange
            // 1. Create product
            var createPayload = new
            {
                title = "Original Title",
                price = 100,
                currency = "USD",
                model = "Test Model Update 2",
                brand = "Test Brand",
                condition = "New",
                stock = 10
            };
            var createRes = await _client.SendAsync(CreatePostRequest("/api/products", createPayload, Guid.NewGuid().ToString()));
            createRes.EnsureSuccessStatusCode();
            var id = createRes.Headers.Location?.LocalPath.Split('/').Last();

            // 2. GET to populate cache
            var getRes1 = await _client.GetAsync($"/api/products/{id}");
            getRes1.EnsureSuccessStatusCode();
            var product1 = await DeserializeResponse<ProductDetailResponse>(getRes1);
            Assert.Equal("Original Title", product1?.Title);

            // Act - 3. Update product
            var updatePayload = new
            {
                title = "Updated Title",
                price = 150,
                currency = "USD",
                model = "Test Model Update 3",
                brand = "Test Brand",
                condition = "New",
                stock = 20
            };
            var content = CreateJsonContent(updatePayload);
            var putRes = await _client.PutAsync($"/api/products/{id}", content);

            // Assert - 4. Check status
            Assert.Equal(HttpStatusCode.NoContent, putRes.StatusCode);

            // Act - 5. GET again (should fetch new data, invalidating cache)
            var getRes2 = await _client.GetAsync($"/api/products/{id}");
            getRes2.EnsureSuccessStatusCode();
            var product2 = await DeserializeResponse<ProductDetailResponse>(getRes2);
            
            Assert.Equal("Updated Title", product2?.Title);
            Assert.Equal(150, product2?.Price);
        }
    }
}
