using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
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
            services.AddSingleton(sp =>
            {
                var cfg = new MapperConfiguration(c =>
                {
                    c.AddProfile(new ProductDetailProfile());
                }, NullLoggerFactory.Instance);

                return cfg.CreateMapper();
            });

            services.AddScoped<IGetProductDetailHandler, GetProductDetailHandler>();
            services.AddScoped<ICreateProductHandler, CreateProductHandler>();
            services.AddScoped<IUpdateProductHandler, UpdateProductHandler>();
            services.AddScoped<IInactivateProductHandler, InactivateProductHandler>();
            services.AddScoped<ISearchProductsHandler, SearchProductsHandler>();
            
            return services;
        }
    }
}
