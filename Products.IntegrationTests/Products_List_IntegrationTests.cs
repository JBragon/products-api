using System.Net;
using Xunit;

namespace Products.IntegrationTests
{
    public class Products_List_IntegrationTests : IntegrationTestBase
    {
        public Products_List_IntegrationTests(ApiFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetList_ShouldReturn200AndDefaultList()
        {
            // Act
            var response = await _client.GetAsync("/api/products");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await DeserializeResponse<ProductListResponse>(response);
            Assert.NotNull(content);
            Assert.True(content.Total >= 0);
            Assert.NotEmpty(content.Items);
            Assert.Equal(1, content.Page);
            // Assuming default pageSize is 10 or 20, we just check items count is within logical limit
            Assert.True(content.Items.Count <= content.PageSize);
        }

        [Fact]
        public async Task GetList_ShouldClampPageSize()
        {
            // Act - requesting pageSize=500
            var response = await _client.GetAsync("/api/products?page=1&pageSize=500");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await DeserializeResponse<ProductListResponse>(response);
            Assert.NotNull(content);
            Assert.Equal(50, content.PageSize); // Max limit is 50
            Assert.True(content.Items.Count <= 50);
        }

        [Fact]
        public async Task GetList_ShouldFilterByBrand()
        {
            // Arrange - grab a brand from the list
            var listResponse = await _client.GetAsync("/api/products?page=1&pageSize=1");
            var listContent = await DeserializeResponse<ProductListResponse>(listResponse);
            
            if (listContent?.Items.Any() != true) return; // Skip if no data

            var expectedBrand = listContent.Items[0].Brand;

            // Act
            var response = await _client.GetAsync($"/api/products?brand={expectedBrand}");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await DeserializeResponse<ProductListResponse>(response);
            Assert.NotNull(content);
            Assert.All(content.Items, item => Assert.Equal(expectedBrand, item.Brand));
        }

        [Fact]
        public async Task GetList_ShouldFilterByCondition()
        {
            // Arrange - grab a condition from the list
            var listResponse = await _client.GetAsync("/api/products?page=1&pageSize=1");
            var listContent = await DeserializeResponse<ProductListResponse>(listResponse);

            if (listContent?.Items.Any() != true) return; // Skip if no data

            var expectedCondition = listContent.Items[0].Condition;

            // Act
            var response = await _client.GetAsync($"/api/products?condition={expectedCondition}");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await DeserializeResponse<ProductListResponse>(response);
            Assert.NotNull(content);
            Assert.All(content.Items, item => Assert.Equal(expectedCondition, item.Condition));
        }

        [Fact]
        public async Task GetList_ShouldSearchByText()
        {
            // Arrange - grab a term from title
            var listResponse = await _client.GetAsync("/api/products?page=1&pageSize=1");
            var listContent = await DeserializeResponse<ProductListResponse>(listResponse);

            if (listContent?.Items.Any() != true) return; // Skip if no data

            var title = listContent.Items[0].Title;
            var term = title.Split(' ').FirstOrDefault() ?? title;

            // Act
            var response = await _client.GetAsync($"/api/products?q={term}");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await DeserializeResponse<ProductListResponse>(response);
            Assert.NotNull(content);
            Assert.NotEmpty(content.Items); // Expect at least one match
            
            // Check that at least one item contains the term in title (approximate check)
            var matches = content.Items.Count(i => i.Title.Contains(term, StringComparison.OrdinalIgnoreCase));
            Assert.True(matches > 0, "No items matched the search term.");
        }
    }
}
