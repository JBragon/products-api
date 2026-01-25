# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ["Products.Api/Products.Api.csproj", "Products.Api/"]
COPY ["Products.Application/Products.Application.csproj", "Products.Application/"]
COPY ["Products.Domain/Products.Domain.csproj", "Products.Domain/"]
COPY ["Products.Infrastructure/Products.Infrastructure.csproj", "Products.Infrastructure/"]

RUN dotnet restore "Products.Api/Products.Api.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/Products.Api"
RUN dotnet publish "Products.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Products.Api.dll"]
