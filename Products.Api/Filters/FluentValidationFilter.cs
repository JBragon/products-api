using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Products.Api.Filters
{
    public sealed class FluentValidationFilter<T> : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ActionArguments.TryGetValue("request", out var arg) || arg is not T model)
            {
                await next();
                return;
            }

            var validator = context.HttpContext.RequestServices.GetService<IValidator<T>>();
            if (validator is null)
            {
                await next();
                return;
            }

            var result = await validator.ValidateAsync(model, context.HttpContext.RequestAborted);
            if (result.IsValid)
            {
                await next();
                return;
            }

            var errors = result.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );

            context.Result = new UnprocessableEntityObjectResult(new ValidationProblemDetails(errors)
            {
                Title = "Validation failed",
                Status = StatusCodes.Status422UnprocessableEntity
            });
        }
    }
}
