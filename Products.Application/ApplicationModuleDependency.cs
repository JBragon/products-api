using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Products.Application.Products.Mapping;
using Products.Application.Products.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Application
{
    public static class ApplicationModuleDependency
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var configExpression = new MapperConfigurationExpression();
            configExpression.AddProfile(new ProductDetailProfile());

            var mapperConfig = new MapperConfiguration(configExpression, NullLoggerFactory.Instance);

            services.AddSingleton<IMapper>(sp =>
                mapperConfig.CreateMapper());

            services.AddScoped<IProductQueryService, ProductQueryService>();

            return services;
        }
    }
}
