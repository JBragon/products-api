using Microsoft.EntityFrameworkCore;
using Products.Api.Data;
using Products.Application;
using Products.Domain.Entities.Products;
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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
