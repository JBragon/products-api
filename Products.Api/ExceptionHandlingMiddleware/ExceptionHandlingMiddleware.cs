using Microsoft.AspNetCore.Mvc;
using Products.Application.Common.Exceptions;

namespace Products.Api.ExceptionHandlingMiddleware
{
    public sealed class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ProductNotFoundException ex)
            {
                _logger.LogWarning("Product Not Found: {Message}", ex.Message);
                context.Response.StatusCode = StatusCodes.Status404NotFound;

                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Product not found",
                    Detail = ex.Message
                };

                await context.Response.WriteAsJsonAsync(problem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred.");
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Internal Server Error",
                    Detail = "An unexpected error occurred."
                };

                await context.Response.WriteAsJsonAsync(problem);
            }
        }
    }
}
