# Use the official ASP.NET runtime image as base
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["CatalogService/CatalogService.csproj", "CatalogService/"]
RUN dotnet restore "CatalogService/CatalogService.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/CatalogService"
RUN dotnet build "CatalogService.csproj" -c Release -o /app/build

# Publish the app to a folder
FROM build AS publish
RUN dotnet publish "CatalogService.csproj" -c Release -o /app/publish

# Final stage/image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CatalogService.dll"]
