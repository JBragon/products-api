using FluentValidation;
using Products.Api.Data;
using Products.Api.ExceptionHandlingMiddleware;
using Products.Api.Filters;
using Products.Api.Validators;
using Products.Application;
using Products.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Application
builder.Services.AddApplication();

// Infrastructure
builder.Services.AddInfrastructure();

// Controllers / Minimal API
builder.Services.AddControllers();

builder.Services.AddMemoryCache();

//Validators
builder.Services.AddValidatorsFromAssemblyContaining<ProductCreateRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<ProductUpdateRequestValidator>();

builder.Services.AddScoped(typeof(FluentValidationFilter<>));


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
    var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

    await ProductJsonSeeder.SeedAsync(db, env);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
