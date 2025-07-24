# 🚀 ProjectAlpha Microservices Estate

_All systems nominal._

---

## 🧱 Architecture Overview

A distributed microservices solution for booking, catalog, authentication, notifications, and API routing — running on Docker containers with healthcheck-aware orchestration and shared infrastructure.

### Services

| Service         | Role                        | Port    | Healthcheck Endpoint         |
|-----------------|-----------------------------|---------|------------------------------|
| BookingService  | Manages user bookings       | 5001    | `/health`                    |
| CatalogService  | Product data & inventory     | 5002    | `/products/health`           |
| ApiGateway      | Routes external requests     | 5000    | `/health`                    |
| UserService     | Authentication & profiles    | 5003    | `/user/health`               |
| EmailService    | Sends notifications          | 5004    | `/email/health`               |
| RabbitMQ        | Message broker               | 5672    | `rabbitmqctl status`         |
| Postgres        | Database                     | 5432    | Native health monitoring     |

---
## 📦 Base Image

All services inherit from the shared base image:

docker build -t projectalpha-base:8.0 ./BaseImage


```dockerfile
FROM projectalpha-base:8.0
```

## 🩺 Healthchecks

Each service exposes a lightweight `/health` endpoint internally. Docker Compose leverages `CMD-SHELL` style healthchecks for accuracy:

```yaml
healthcheck:
  test: ["CMD-SHELL", "curl -f http://localhost:8080/health || exit 1"]
  interval: 30s
  timeout: 5s
  retries: 3

docker-compose down
docker-compose build
docker-compose up -d

docker-compose build gateway
docker-compose up -d gateway
```

### Messaging Infrastructure Startup

To ensure our messaging layer initializes automatically with application startup, we register a custom `IHostedService`:

```csharp
public class MessagingStartupHostedService(IServiceProvider provider) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = provider.CreateScope();
        var publisher = scope.ServiceProvider.GetRequiredService<IMessagePublisher>();
        await publisher.SetupAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}

