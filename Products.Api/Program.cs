using Microsoft.EntityFrameworkCore;
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

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

    if (!await db.Products.AnyAsync())
    {
        var p = new Product(
            id: Guid.Parse("11111111-1111-1111-1111-111111111111"),
            title: "iPhone 13 128GB",
            condition: ProductCondition.New,
            price: new Money(4500m, "BRL"),
            stock: new Stock(12),
            description: "Produto lacrado."
        );

        db.Products.Add(p);
        await db.SaveChangesAsync();
    }
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
