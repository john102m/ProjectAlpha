# Use the official ASP.NET Core runtime as the base
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Set working directory for all services
WORKDIR /app

# Install curl for healthchecks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Optional: Set default timezone
ENV TZ=Europe/London

