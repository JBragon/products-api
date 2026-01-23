using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Products.Application.Common.Caching;
using Products.Application.Products.Ports;
using Products.Infrastructure.Caching;
using Products.Infrastructure.Idempotency;
using Products.Infrastructure.Repositories;

namespace Products.Infrastructure
{
    public static class InfrastructureModuleDependency
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddDbContext<DatabaseContext>(options =>
            {
                options.UseInMemoryDatabase("products-db");
            });

            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IIdempotencyStore, InMemoryIdempotencyStore>();
            services.AddSingleton<IProductCache, ProductMemoryCache>();

            return services;
        }

    }
}
