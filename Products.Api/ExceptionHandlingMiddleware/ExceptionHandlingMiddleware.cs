using Microsoft.AspNetCore.Mvc;
using Products.Application.Common.Exceptions;

namespace Products.Api.ExceptionHandlingMiddleware
{
    public sealed class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
            => _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ProductNotFoundException ex)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;

                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Product not found",
                    Detail = ex.Message
                };

                await context.Response.WriteAsJsonAsync(problem);
            }
        }
    }
}
