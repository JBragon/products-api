using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Products.Application.Common.Caching;
using Products.Application.Products.Mapping;
using Products.Application.Products.UseCases.Create;
using Products.Application.Products.UseCases.Delete;
using Products.Application.Products.UseCases.Query.GetDetail;
using Products.Application.Products.UseCases.Query.Search;
using Products.Application.Products.UseCases.Update;

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

            services.AddScoped<IGetProductDetailHandler, GetProductDetailHandler>();
            services.AddScoped<ICreateProductHandler, CreateProductHandler>();
            services.AddScoped<IUpdateProductHandler, UpdateProductHandler>();
            services.AddScoped<IInactivateProductHandler, InactivateProductHandler>();
            services.AddScoped<IGetProductDetailHandler, GetProductDetailHandler>();
            services.AddScoped<ISearchProductsHandler, SearchProductsHandler>();
            services.AddSingleton<IProductCache, ProductMemoryCache>();

            return services;
        }
    }
}
