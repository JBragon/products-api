using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Products.Application.Common.Caching;
using Products.Application.Products.Mapping;
using Products.Application.Products.Services.Command;
using Products.Application.Products.Services.Query;

namespace Products.Application
{
    public static class ApplicationModuleDependency
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var configExpression = new MapperConfigurationExpression();
            configExpression.AddProfile(new ProductDetailProfile());

            var mapperConfig = new MapperConfiguration(configExpression, NullLoggerFactory.Instance);

            services.AddSingleton(sp =>
                mapperConfig.CreateMapper());

            services.AddScoped<IProductQueryService, ProductQueryService>();
            services.AddScoped<ICreateProductHandler, CreateProductHandler>();
            services.AddScoped<IUpdateProductHandler, UpdateProductHandler>();
            services.AddScoped<IInactivateProductHandler, InactivateProductHandler>();
            services.AddSingleton<IProductCache, ProductMemoryCache>();

            return services;
        }
    }
}
