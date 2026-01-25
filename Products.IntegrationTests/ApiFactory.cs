using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Products.Infrastructure;

namespace Products.IntegrationTests
{
    public class ApiFactory : WebApplicationFactory<Program>
    {
        private readonly string _dbName = $"products-db-test-{Guid.NewGuid()}";

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<DatabaseContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<DatabaseContext>(options =>
                {
                    options.UseInMemoryDatabase(_dbName);
                });

                // Debug logic removed
                
            });
        }
    }
}
