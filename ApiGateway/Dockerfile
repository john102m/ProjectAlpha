# Use the official ASP.NET runtime image as base
# Replace this:
# FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

# With your custom base image:
FROM projectalpha-base:8.0 AS base

WORKDIR /app
EXPOSE 80

# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy everything else and build
COPY . .
WORKDIR "/src/ApiGateway"

RUN dotnet restore "ApiGateway.csproj"
RUN dotnet build "ApiGateway.csproj" -c Release -o /app/build

# Publish the app to a folder
FROM build AS publish
RUN dotnet publish "ApiGateway.csproj" -c Release -o /app/publish

# Final stage/image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ApiGateway.dll"]
