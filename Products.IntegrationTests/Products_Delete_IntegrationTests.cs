using System.Net;
using Xunit;

namespace Products.IntegrationTests
{
    public class Products_Delete_IntegrationTests : IntegrationTestBase
    {
        public Products_Delete_IntegrationTests(ApiFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Delete_ShouldReturn404_WhenProductDoesNotExist()
        {
            // Act
            var response = await _client.DeleteAsync($"/api/products/{Guid.NewGuid()}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Delete_ShouldReturn204AndInvalidateCache_WhenDeleteIsValid()
        {
            // Arrange
            // 1. Create product
            var createPayload = new
            {
                title = "To Be Deleted",
                price = 100,
                currency = "USD",
                model = "Test Model Delete 1",
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

            // Act - 3. Delete
            var deleteRes = await _client.DeleteAsync($"/api/products/{id}");

            // Assert - 4. Check status
            Assert.Equal(HttpStatusCode.NoContent, deleteRes.StatusCode);

            // Act - 5. GET again (should be 404, proving cache invalidation and soft delete)
            var getRes2 = await _client.GetAsync($"/api/products/{id}");
            Assert.Equal(HttpStatusCode.NotFound, getRes2.StatusCode);
        }

        [Fact]
        public async Task Delete_ShouldRemoveFromList()
        {
             // Arrange
            // 1. Create product with unique title
            var uniqueTitle = $"DeleteTest-{Guid.NewGuid()}";
            var createPayload = new
            {
                title = uniqueTitle,
                price = 100,
                currency = "USD",
                model = "Test Model Delete 2",
                brand = "Test Brand",
                condition = "New",
                stock = 10
            };
            var createRes = await _client.SendAsync(CreatePostRequest("/api/products", createPayload, Guid.NewGuid().ToString()));
            createRes.EnsureSuccessStatusCode();
            var id = createRes.Headers.Location?.LocalPath.Split('/').Last();

            // 2. Verify it shows in list (Filter by search q)
            var listRes1 = await _client.GetAsync($"/api/products?q={uniqueTitle}");
            listRes1.EnsureSuccessStatusCode();
            var listContent1 = await DeserializeResponse<ProductListResponse>(listRes1);
            Assert.True(listContent1?.Items.Any(i => i.Id.ToString() == id));

            // Act - 3. Delete
            var deleteRes = await _client.DeleteAsync($"/api/products/{id}");
            Assert.Equal(HttpStatusCode.NoContent, deleteRes.StatusCode);

            // Assert - 4. Verify it does NOT show in list (Filter by search q)
            var listRes2 = await _client.GetAsync($"/api/products?q={uniqueTitle}");
            listRes2.EnsureSuccessStatusCode();
            var listContent2 = await DeserializeResponse<ProductListResponse>(listRes2);
            
            // Should be empty or not contain the ID
            if (listContent2 != null && listContent2.Items.Any())
            {
                var matches = listContent2.Items.Count(i => i.Id.ToString() == id);
                Assert.Equal(0, matches);
            }
        }
    }
}
