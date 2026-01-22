using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Products.Application.Common.Idempotency;
using Products.Application.Ports;
using Products.Infrastructure.Idempotency;
using Products.Infrastructure.Repositories;

namespace Products.Infrastructure
{
    public static class InfraestructureModuleDependency
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddDbContext<DatabaseContext>(options =>
            {
                options.UseInMemoryDatabase("products-db");
            });

            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IIdempotencyStore, InMemoryIdempotencyStore>();

            return services;
        }

    }
}
